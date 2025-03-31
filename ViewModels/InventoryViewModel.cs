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
using kafi.Models.Inventory;
using kafi.Repositories;

namespace kafi.ViewModels
{
    [ObservableRecipient]
    public partial class InventoryViewModel(IInventoryRepository repository) : ObservableValidator
    {
        private readonly IInventoryRepository _repository = repository;
        private const int DefaultPageSize = 10;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(TurnOnEditingCommand))]
        private bool isEditing = false;

        public Guid EditingId { get; private set; } = Guid.Empty;

        [ObservableProperty]
        [Required]
        private string name;

        [ObservableProperty]
        [Required]
        private int originalStock;

        [ObservableProperty]
        [Required]
        private int currentStock;

        [ObservableProperty]
        [Required]
        private string unit;

        [ObservableProperty]
        [Required]
        private DateTimeOffset expiredDate = DateTime.Now.AddYears(1);

        [ObservableProperty]
        [Required]
        private int price;

        [ObservableProperty]
        private string message = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(GoToPreviousPageCommand))]
        [NotifyCanExecuteChangedFor(nameof(GoToNextPageCommand))]
        private int currentPage = 1;

        [ObservableProperty]
        private int totalPages = 1;

        [ObservableProperty]
        private int pageSize = DefaultPageSize;

        [ObservableProperty]
        private int totalItems = 0;

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

            if (HasErrors)
            {
                Message = "Please fix the errors above.";
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
                Message = "Inventory added successfully.";
                DeleteAllInput();
                WeakReferenceMessenger.Default.Send(new PropertyChangedMessage<object>(this, nameof(Inventories), null, _fullInventoryList));
            }
            catch (Exception ex)
            {
                Message = $"Error adding inventory";
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
            if (HasErrors)
            {
                Message = "Please fix the errors above.";
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
                WeakReferenceMessenger.Default.Send(new ValueChangedMessage<string>(Message));
                WeakReferenceMessenger.Default.Send(new PropertyChangedMessage<object>(this, nameof(Inventories), null, _fullInventoryList));
            }
            catch (Exception ex)
            {
                Message = $"Error updating inventory";
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
                Message = string.Empty;
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

    }
}
