using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using kafi.Models;
using kafi.Repositories;
using Microsoft.UI.Dispatching;

namespace kafi.ViewModels;

[ObservableRecipient]
public partial class InventoryViewModel : ObservableValidator, IRecipient<ValueChangedMessage<string>>
{
    private readonly IInventoryRepository _repository;
    private const int DefaultPageSize = 10;
    private System.Timers.Timer? _successMessageTimer;
    private DispatcherQueue _dispatcherQueue;

    public InventoryViewModel(IInventoryRepository repository)
    {
        _repository = repository;
        Messenger = WeakReferenceMessenger.Default;
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        IsActive = true;

        // Initialize the timer
        _successMessageTimer = new System.Timers.Timer(3000); // 3 seconds
        _successMessageTimer.Elapsed += (s, e) =>
        {
            // Dispatch the UI update to the UI thread
            _dispatcherQueue.TryEnqueue(() =>
            {
                SuccessMessage = string.Empty;
            });
            _successMessageTimer.Stop();
        };
        _successMessageTimer.AutoReset = false;
    }

    [ObservableProperty]
    public partial bool IsLoading { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(TurnOnEditingCommand))]
    public partial bool IsEditing { get; set; } = false;
    public Guid EditingId { get; private set; } = Guid.Empty;

    [ObservableProperty]
    [Required]
    public partial string? Name { get; set; }

    [ObservableProperty]
    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Khối lượng đầu vào phải là số dương")]
    public partial int OriginalStock { get; set; }

    [ObservableProperty]
    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Tồn kho không được âm")]
    public partial int CurrentStock { get; set; } = 0;

    [ObservableProperty]
    [Required]
    public partial string? Unit { get; set; }

    [ObservableProperty]
    [Required]
    public partial DateTimeOffset ExpiredDate { get; set; } = DateTime.Now.AddYears(1);

    [ObservableProperty]
    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Giá phải là số dương")]
    public partial int Price { get; set; }

    [ObservableProperty]
    public partial string ErrorMessage { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string SuccessMessage { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(GoToPreviousPageCommand))]
    [NotifyCanExecuteChangedFor(nameof(GoToNextPageCommand))]
    public partial int CurrentPage { get; set; } = 1;

    [ObservableProperty]
    public partial int TotalPages { get; set; } = 1;

    [ObservableProperty]
    public partial int PageSize { get; set; } = DefaultPageSize;

    [ObservableProperty]
    public partial int TotalItems { get; set; } = 0;

    private List<Inventory> _fullInventoryList = [];
    public ObservableCollection<Inventory> Inventories { get; } = new ObservableCollection<Inventory>();

    private bool CanLoadData => !Inventories.Any();
    [RelayCommand(CanExecute = nameof(CanLoadData))]
    private async Task LoadDataAsync()
    {
        IsLoading = true;
        try
        {
            _fullInventoryList = [.. await _repository.GetAll()];
            SortInventoryByNewestUpdate();
            UpdatePagedView();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading inventory: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task AddInventoryAsync()
    {
        ValidateAllProperties();
        SuccessMessage = string.Empty;

        if (HasErrors)
        {
            var errors = GetErrors();
            if (errors.Any())
            {
                ErrorMessage = errors.First().ErrorMessage!;
            }
            else
            {
                ErrorMessage = "Vui lòng sửa các lỗi bên trên.";
            }
            return;
        }

        var inventory = new InventoryRequest
        {
            Name = Name,
            OriginalStock = OriginalStock,
            Unit = Unit,
            ExpiredDate = ExpiredDate.DateTime,
            Price = Price
        };

        try
        {
            Inventory newInventory = (Inventory)await _repository.Add(inventory) ?? throw new Exception();
            _fullInventoryList.Add(newInventory);
            UpdatePagedView();
            ErrorMessage = string.Empty;
            DeleteAllInput();
            SuccessMessage = "Thêm mặt hàng thành công.";
            WeakReferenceMessenger.Default.Send(new PropertyChangedMessage<object>(this, nameof(Inventories), null, _fullInventoryList));
        }
        catch (Exception ex)
        {
            ErrorMessage = "Lỗi khi thêm mặt hàng";
            SuccessMessage = string.Empty;
        }
    }

    private bool CanTurnOnEditing(Guid id) => id != Guid.Empty && !IsEditing;
    [RelayCommand(CanExecute = nameof(CanTurnOnEditing))]
    private void TurnOnEditing(Guid id)
    {
        var inventory = _fullInventoryList.FirstOrDefault(u => u.Id == id);
        if (inventory == null)
        {
            return;
        }
        Name = inventory.Name;
        OriginalStock = inventory.OriginalStock;
        CurrentStock = inventory.CurrentStock;
        Unit = inventory.Unit;
        ExpiredDate = inventory.ExpiredDate;
        Price = inventory.Price;

        IsEditing = true;
        EditingId = id;
        UpdateInventoryCommand.NotifyCanExecuteChanged();
    }

    private bool CanUpdateInventory() => IsEditing && EditingId != Guid.Empty;
    [RelayCommand(CanExecute = nameof(CanUpdateInventory))]
    private async Task UpdateInventoryAsync()
    {
        ValidateAllProperties();
        SuccessMessage = string.Empty;

        // Additional validation for CurrentStock
        if (CurrentStock > OriginalStock)
        {
            ErrorMessage = "Tồn kho không được lớn hơn khối lượng đầu vào";
            return;
        }

        if (HasErrors)
        {
            var errors = GetErrors();
            if (errors.Any())
            {
                ErrorMessage = errors.First().ErrorMessage!;
            }
            else
            {
                ErrorMessage = "Vui lòng sửa các lỗi bên trên.";
            }
            return;
        }
        var inventory = new InventoryRequest
        {
            Name = Name,
            OriginalStock = OriginalStock,
            Unit = Unit,
            ExpiredDate = ExpiredDate.DateTime,
            Price = Price
        };
        try
        {
            await _repository.Update(EditingId, inventory);
            var index = _fullInventoryList.FindIndex(u => u.Id == EditingId);
            if (index != -1)
            {
                _fullInventoryList[index] = new Inventory
                {
                    Id = EditingId,
                    Name = Name,
                    OriginalStock = OriginalStock,
                    CurrentStock = _fullInventoryList[index].CurrentStock,
                    Unit = Unit,
                    ExpiredDate = ExpiredDate.DateTime,
                    Price = Price,
                    CreatedAt = _fullInventoryList[index].CreatedAt,
                    UpdatedAt = DateTimeOffset.Now.DateTime
                };
            }
            UpdatePagedView();
            SuccessMessage = "Cập nhật mặt hàng thành công.";
            ErrorMessage = string.Empty;
            WeakReferenceMessenger.Default.Send(new ValueChangedMessage<string>(SuccessMessage));
            WeakReferenceMessenger.Default.Send(new PropertyChangedMessage<object>(this, nameof(Inventories), null, _fullInventoryList));
        }
        catch (Exception ex)
        {
            ErrorMessage = "Lỗi khi cập nhật mặt hàng";
            SuccessMessage = string.Empty;
        }
    }

    [RelayCommand]
    private async Task DeleteInventoryAsync(Guid id)
    {
        try
        {
            await _repository.Delete(id);
            var removedCount = _fullInventoryList.RemoveAll(u => u.Id == id);

            if (removedCount > 0 &&
                Inventories.Count == 1 &&
                CurrentPage > 1)
            {
                CurrentPage--;
            }

            UpdatePagedView();
            WeakReferenceMessenger.Default.Send(new PropertyChangedMessage<object>(this, nameof(Inventories), null, _fullInventoryList));
        }
        catch (Exception) { }
    }

    [RelayCommand]
    private void DeleteAllInput()
    {
        if (IsEditing)
        {
            var inventory = _fullInventoryList.FirstOrDefault(u => u.Id == EditingId);
            if (inventory == null)
            {
                return;
            }
            Name = inventory.Name;
            OriginalStock = inventory.OriginalStock;
            CurrentStock = inventory.CurrentStock;
            Unit = inventory.Unit;
            ExpiredDate = inventory.ExpiredDate;
            Price = inventory.Price;
        }
        else
        {
            Name = string.Empty;
            Price = 0;
            Unit = string.Empty;
            OriginalStock = 0;
            ExpiredDate = DateTimeOffset.Now.AddYears(1);
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;
        }
    }

    private void UpdatePagedView()
    {
        TotalItems = _fullInventoryList.Count;
        CurrentPage = Math.Clamp(CurrentPage, 1, TotalPages);

        TotalPages = (int)Math.Ceiling((double)TotalItems / PageSize);
        TotalPages = TotalPages == 0 ? 1 : TotalPages;

        var pagedItems = _fullInventoryList
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize);

        Inventories.Clear();
        foreach (var item in pagedItems)
        {
            Inventories.Add(item);
        }

        GoToPreviousPageCommand.NotifyCanExecuteChanged();
        GoToNextPageCommand.NotifyCanExecuteChanged();
    }

    private bool CanGoToPreviousPage => CurrentPage > 1;
    [RelayCommand(CanExecute = nameof(CanGoToPreviousPage))]
    private void GoToPreviousPage()
    {
        CurrentPage--;
        UpdatePagedView();
    }

    private bool CanGoToNextPage => CurrentPage < TotalPages;
    [RelayCommand(CanExecute = nameof(CanGoToNextPage))]
    private void GoToNextPage()
    {
        CurrentPage++;
        UpdatePagedView();
    }

    partial void OnPageSizeChanged(int value)
    {
        UpdatePagedView();
    }

    private void SortInventoryByNewestUpdate()
    {
        _fullInventoryList = _fullInventoryList
            .OrderByDescending(inv => inv.UpdatedAt)
            .ToList();
    }

    public async void Receive(ValueChangedMessage<string> message)
    {
        if (message.Value == "ordercreated")
        {
            await LoadDataAsync();
            WeakReferenceMessenger.Default.Send(new ValueChangedMessage<string>("materialupdated"));
        }
    }

    partial void OnSuccessMessageChanged(string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            _successMessageTimer?.Start();
        }
    }

    [ObservableProperty]
    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Số lượng cập nhật không được âm")]
    [NotifyCanExecuteChangedFor(nameof(UpdateCurrentStockCommand))]
    public partial int StockUpdateAmount { get; set; } = 0;

    private bool CanUpdateCurrentStock() => EditingId != Guid.Empty;
    [RelayCommand(CanExecute = nameof(CanUpdateCurrentStock))]
    private async Task UpdateCurrentStockAsync()
    {
        ValidateAllProperties();
        SuccessMessage = string.Empty;

        if (HasErrors)
        {
            var errors = GetErrors();
            if (errors.Any())
            {
                ErrorMessage = errors.First().ErrorMessage!;
            }
            else
            {
                ErrorMessage = "Vui lòng sửa các lỗi bên trên.";
            }
            return;
        }

        try
        {
            // Directly use the input value instead of adding/subtracting
            int newStockValue = StockUpdateAmount;

            if (newStockValue > OriginalStock)
            {
                ErrorMessage = "Tồn kho không thể lớn hơn khối lượng đầu vào";
                return;
            }

            await _repository.UpdateCurrentStock(EditingId, newStockValue);
            var index = _fullInventoryList.FindIndex(u => u.Id == EditingId);
            if (index != -1)
            {
                _fullInventoryList[index] = new Inventory
                {
                    Id = EditingId,
                    Name = _fullInventoryList[index].Name,
                    OriginalStock = _fullInventoryList[index].OriginalStock,
                    CurrentStock = newStockValue,
                    Unit = _fullInventoryList[index].Unit,
                    ExpiredDate = _fullInventoryList[index].ExpiredDate,
                    Price = _fullInventoryList[index].Price,
                    CreatedAt = _fullInventoryList[index].CreatedAt,
                    UpdatedAt = DateTimeOffset.Now.DateTime
                };
            }
            UpdatePagedView();
            SuccessMessage = "Cập nhật tồn kho thành công.";
            ErrorMessage = string.Empty;
            StockUpdateAmount = 0;
            WeakReferenceMessenger.Default.Send(new ValueChangedMessage<string>(SuccessMessage));
            WeakReferenceMessenger.Default.Send(new PropertyChangedMessage<object>(this, nameof(Inventories), null, _fullInventoryList));
            WeakReferenceMessenger.Default.Send(new ValueChangedMessage<string>("materialupdated"));
        }
        catch (Exception ex)
        {
            ErrorMessage = "Lỗi khi cập nhật tồn kho";
            SuccessMessage = string.Empty;
        }
    }
}
