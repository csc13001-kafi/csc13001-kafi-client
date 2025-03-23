using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using kafi.Models;
using kafi.Repositories;

namespace kafi.ViewModels
{
    public partial class InventoryViewModel : ObservableValidator
    {
        private readonly IInventoryRepository _repository;
        private const int DefaultPageSize = 10;

        public ObservableCollection<Inventory> Inventories { get; } = new ObservableCollection<Inventory>();

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        [Required]
        private string name;

        [ObservableProperty]
        [Required]
        private double originalStock;

        [ObservableProperty]
        [Required]
        private double currentStock;

        [ObservableProperty]
        [Required]
        private string unit;

        [ObservableProperty]
        [Required]
        private DateTime expiredDate = DateTime.Now.AddYears(1);

        [ObservableProperty]
        [Required]
        private decimal price;

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

        private List<Inventory> _fullInventoryList = new();
        
        public InventoryViewModel(IInventoryRepository repository)
        {
            _repository = repository;
            // Initialize with the first page
            CurrentPage = 1;
            PageSize = 10;
            _ = LoadDataAsync();
        }

        private void UpdatePagedView(List<Inventory> allItems)
        {
            TotalItems = allItems.Count;
            TotalPages = (int)Math.Ceiling((double)TotalItems / PageSize);
            
            if (CurrentPage < 1) CurrentPage = 1;
            if (CurrentPage > TotalPages && TotalPages > 0) CurrentPage = TotalPages;

            Inventories.Clear();
            var pagedItems = allItems
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

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
            UpdatePagedView(_fullInventoryList);
        }

        private bool CanGoToNextPage => CurrentPage < TotalPages;
        [RelayCommand(CanExecute = nameof(CanGoToNextPage))]
        private void GoToNextPage()
        {
            CurrentPage++;
            UpdatePagedView(_fullInventoryList);
        }

        partial void OnPageSizeChanged(int value)
        {
            UpdatePagedView(_fullInventoryList);
        }

        partial void OnCurrentPageChanged(int value)
        {
            UpdatePagedView(_fullInventoryList);
        }

        [RelayCommand]
        private async Task LoadDataAsync()
        {
            IsLoading = true;
            try
            {
                _fullInventoryList = (await _repository.GetAll()).ToList();
                TotalItems = _fullInventoryList.Count;
                UpdatePagedView(_fullInventoryList);
                Message = $"Loaded {TotalItems} inventory items";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading inventory: {ex.Message}");
                Message = $"Error loading inventory: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task AddInventoryAsync()
        {
            throw new NotImplementedException();
        }

        [RelayCommand]
        private async Task UpdateInventoryAsync()
        {
            throw new NotImplementedException();
        }

        [RelayCommand]
        private async Task DeleteInventoryAsync(Inventory inventory)
        {
            throw new NotImplementedException();
        }

        [RelayCommand]
        private void ClearForm()
        {
            Name = string.Empty;
            OriginalStock = 0;
            CurrentStock = 0;
            Unit = string.Empty;
            ExpiredDate = DateTime.Now.AddYears(1);
            Price = 0;
            Message = string.Empty;
        }

        [RelayCommand]
        private Task SelectInventoryAsync(Inventory inventory)
        {
            throw new NotImplementedException();
        }
    }
}
