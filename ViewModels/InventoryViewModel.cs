using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using kafi.Contracts.Services;
using kafi.Models;
using kafi.Repositories;

namespace kafi.ViewModels
{
    public partial class InventoryViewModel(IInventoryRepository repository, IAuthService authService) : ObservableValidator
    {
        private readonly IInventoryRepository _repository = repository;
        private readonly IAuthService _authService = authService;
        private const int DefaultPageSize = 10;
        public bool IsManager => _authService.IsInRole(Role.Manager);

        [ObservableProperty]
        private bool isLoading;

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

            var inventory = new Inventory
            {
                Name = Name,
                OriginalStock = OriginalStock,
                CurrentStock = CurrentStock,
                Unit = Unit,
                ExpiredDate = ExpiredDate.DateTime,
                Price = Price
            };

            try
            {
                await _repository.Add(inventory);
                _fullInventoryList.Add(inventory);
                UpdatePagedView();
                Message = "Inventory added successfully.";
                DeleteAllInput();
            }
            catch (Exception ex)
            {
                Message = $"Error adding inventory: {ex.Message}";
            }
        }

        [RelayCommand]
        private async Task UpdateInventoryAsync()
        {
            throw new NotImplementedException();
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
            }
            catch (Exception) { }
        }

        [RelayCommand]
        private void DeleteAllInput()
        {
            Name = string.Empty;
            Price = 0;
            Unit = string.Empty;
            OriginalStock = 0;
            ExpiredDate = DateTimeOffset.Now.AddYears(1);
            Message = string.Empty;
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
