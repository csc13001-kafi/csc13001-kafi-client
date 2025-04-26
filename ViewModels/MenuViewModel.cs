using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using kafi.Contracts.Services;
using kafi.Models;
using kafi.Repositories;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage;

namespace kafi.ViewModels;

// Enum to track the current operation mode
public enum Mode
{
    None,
    AddingCategory,
    EditingCategory,
    AddingProduct,
    EditingProduct
}

public partial class MenuViewModel : ObservableRecipient, IRecipient<PropertyChangedMessage<object>>
{
    private readonly IMenuRepository _repository;
    private readonly IWindowService _windowService;
    private readonly IAuthService _authService;
    private StorageFile? _selectedFile; // Holds the selected image file
    private Guid _selectedItemId = Guid.Empty; // Tracks the ID of the item being edited
    public bool IsEmployee => _authService.IsInRole(Role.Employee);

    // Collections for UI binding
    public ObservableCollection<Category> Categories { get; set; } = new ObservableCollection<Category>();
    public ObservableCollection<Product> FilteredProducts { get; } = new ObservableCollection<Product>();
    public ObservableCollection<ProductMaterial> SelectedMaterials { get; } = new ObservableCollection<ProductMaterial>();
    public ObservableCollection<Category> SelectedCategories { get; } = new ObservableCollection<Category>();

    // Backing collections for full data
    private List<Product> _fullProducts = [];

    public MenuViewModel(IMenuRepository repository, IWindowService windowService, IAuthService authService)
    {
        _repository = repository;
        _windowService = windowService;
        _authService = authService;

        IsActive = true;
    }

    public List<Inventory> FullMaterials { get; set; } = [];

    #region Observable Properties

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddCommand))]
    [NotifyCanExecuteChangedFor(nameof(UpdateCommand))]
    public partial Category? SelectedCategoryForEdit { get; set; } = null;

    [ObservableProperty]
    public partial ProductMaterial? SelectedMaterialForEdit { get; set; } = null;

    [ObservableProperty]
    public partial bool IsLoading { get; set; }

    [ObservableProperty]
    public partial Mode CurrentMode { get; set; } = Mode.None;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddCommand))]
    [NotifyCanExecuteChangedFor(nameof(UpdateCommand))]
    public partial string Name { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddCommand))]
    [NotifyCanExecuteChangedFor(nameof(UpdateCommand))]
    public partial int Price { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddCommand))]
    [NotifyCanExecuteChangedFor(nameof(UpdateCommand))]
    [NotifyPropertyChangedFor(nameof(IsImageSelected))]
    public partial ImageSource? SelectedImage { get; set; }

    public bool IsImageSelected => SelectedImage != null;

    [ObservableProperty]
    public partial string StatusMessage { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string SuccessMessage { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string ErrorMessage { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool HasError { get; set; }

    [ObservableProperty]
    public partial bool HasSuccess { get; set; }
    #endregion

    #region Commands

    private bool CanLoadData => !IsLoading && Categories.Count == 0;
    [RelayCommand(CanExecute = nameof(CanLoadData))]
    private async Task LoadDataAsync()
    {
        IsLoading = true;
        try
        {
            var response = await _repository.GetCategoriesAndProducts();
            _fullProducts = response.Products;

            Categories.Clear();
            foreach (var category in response.Categories)
                Categories.Add(category);

            // Load all products instead of filtering by the first category
            LoadAllProducts();
        }
        catch (Exception ex)
        {
            SetStatusMessage($"Error loading data: {ex.Message}", isError: true);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private bool CanLoadMaterials => !IsLoading && FullMaterials.Count == 0;
    [RelayCommand(CanExecute = nameof(CanLoadMaterials))]
    private async Task LoadMaterialsAsync()
    {
        try
        {
            FullMaterials = [.. (await _repository.GetMaterials())];
            if (FullMaterials.Count != 0 && !SelectedMaterials.Any())
            {
                SelectedMaterials.Add(new ProductMaterial { Id = FullMaterials[0].Id });
            }
        }
        catch (Exception ex)
        {
            SetStatusMessage($"Error loading materials: {ex.Message}", isError: true);
        }
    }

    [RelayCommand]
    private async Task PickImageAsync()
    {
        WeakReferenceMessenger.Default.Send(new ValueChangedMessage<string>("showoverlay"));
        try
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            WinRT.Interop.InitializeWithWindow.Initialize(picker, WinRT.Interop.WindowNative.GetWindowHandle(_windowService.GetCurrentWindow()));

            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            _selectedFile = await picker.PickSingleFileAsync();

            if (_selectedFile != null)
            {
                using var stream = await _selectedFile.OpenAsync(FileAccessMode.Read);
                var bitmapImage = new BitmapImage();
                await bitmapImage.SetSourceAsync(stream);
                SelectedImage = bitmapImage;
            }
        }
        catch (Exception ex)
        {
            SetStatusMessage($"Error picking image: {ex.Message}", isError: true);
        }
        finally
        {
            WeakReferenceMessenger.Default.Send(new ValueChangedMessage<string>("hideoverlay"));
        }
    }

    [RelayCommand]
    private void TurnOnAddingCategory()
    {
        ResetForm();
        CurrentMode = Mode.AddingCategory;
    }

    [RelayCommand]
    private void TurnOnAddingProduct()
    {
        if (!Categories.Any())
        {
            SetStatusMessage("Please add a category first.", isError: true);
            return;
        }
        ResetForm();
        CurrentMode = Mode.AddingProduct;
    }

    [RelayCommand]
    private void TurnOnEditingCategory(Guid id)
    {
        var category = Categories.FirstOrDefault(c => c.Id == id);
        if (category == null) return;

        ResetForm();
        Name = category.Name ?? string.Empty;
        SelectedImage = new BitmapImage(new Uri(category.Image ?? string.Empty));
        _selectedItemId = id;
        CurrentMode = Mode.EditingCategory;
    }

    [RelayCommand]
    private void TurnOnEditingProduct(Guid id)
    {
        var product = _fullProducts.FirstOrDefault(p => p.Id == id);
        if (product == null) return;

        ResetForm();
        Name = product.Name ?? string.Empty;
        SelectedCategoryForEdit = Categories.FirstOrDefault(c => c.Id == product.CategoryId);
        Price = product.Price;

        SelectedMaterials.Clear();
        foreach (var material in product.Materials!)
            SelectedMaterials.Add(new ProductMaterial
            {
                Id = material.Id,
                Quantity = material.Quantity
            });

        SelectedImage = new BitmapImage(new Uri(product.Image ?? string.Empty));
        _selectedItemId = id;

        CurrentMode = Mode.EditingProduct;
    }

    private bool CanAddOrUpdate =>
      !string.IsNullOrWhiteSpace(Name) && (CurrentMode == Mode.AddingCategory || CurrentMode == Mode.EditingCategory ||
        ((CurrentMode == Mode.AddingProduct || CurrentMode == Mode.EditingProduct) && SelectedCategoryForEdit != null && Price > 0));
    [RelayCommand(CanExecute = nameof(CanAddOrUpdate))]
    private async Task AddAsync()
    {
        if (!ValidateInput() || _selectedFile == null) return;

        try
        {
            if (CurrentMode == Mode.AddingCategory)
            {
                var request = new CreateCategoryRequest(Name, _selectedFile.OpenStreamForReadAsync().Result, _selectedFile!.ContentType, _selectedFile.Name);

                var newCategory = await _repository.AddCategory(request);
                Categories.Add(newCategory);

                SetStatusMessage("Category added successfully", isError: false);
                SuccessMessage = "Thêm phân loại thành công.";
                ErrorMessage = string.Empty;
            }
            else if (CurrentMode == Mode.AddingProduct)
            {
                var request = new CreateProductRequest(Name, Price, true, SelectedCategoryForEdit!.Id, [.. SelectedMaterials], _selectedFile.OpenStreamForReadAsync().Result, _selectedFile!.ContentType, _selectedFile.Name);
                var newProduct = await _repository.AddProduct(request);
                newProduct.Materials = [.. SelectedMaterials];

                _fullProducts.Add(newProduct);

                // Apply current category filter to ensure new product shows up if appropriate
                FilterByMultipleCategories(SelectedCategories);

                SetStatusMessage("Product added successfully", isError: false);
                SuccessMessage = "Thêm sản phẩm thành công.";
                ErrorMessage = string.Empty;
            }

            WeakReferenceMessenger.Default.Send(new ValueChangedMessage<string>("close"));
        }
        catch (Exception ex)
        {
            SetStatusMessage($"Error adding item: {ex.Message}", isError: true);
            ErrorMessage = $"Lỗi khi thêm: {ex.Message}";
            SuccessMessage = string.Empty;
        }
    }

    [RelayCommand(CanExecute = nameof(CanAddOrUpdate))]
    private async Task UpdateAsync()
    {
        if (!ValidateInput() || _selectedItemId == Guid.Empty) return;

        try
        {
            if (CurrentMode == Mode.EditingCategory)
            {
                var request = _selectedFile is null ? new CreateCategoryRequest(Name) : new CreateCategoryRequest(Name, _selectedFile.OpenStreamForReadAsync().Result, _selectedFile!.ContentType, _selectedFile.Name);
                await _repository.UpdateCategory(_selectedItemId, request);
                UpdateCategoryInCollections();
                SetStatusMessage("Phân loại đã được cập nhật thành công", isError: false);
                SuccessMessage = "Phân loại đã được cập nhật thành công.";
            }
            else if (CurrentMode == Mode.EditingProduct)
            {
                var request = _selectedFile is null ? new CreateProductRequest(Name, Price, true, SelectedCategoryForEdit!.Id, [.. SelectedMaterials]) : new CreateProductRequest(Name, Price, true, SelectedCategoryForEdit!.Id, SelectedMaterials.ToList(), _selectedFile.OpenStreamForReadAsync().Result, _selectedFile!.ContentType, _selectedFile.Name);
                await _repository.UpdateProduct(_selectedItemId, request);

                UpdateProductInCollections();

                // Apply current category filter after updating
                FilterByMultipleCategories(SelectedCategories);

                SetStatusMessage("Sản phẩm đã được cập nhật thành công", isError: false);
                SuccessMessage = "Sản phẩm đã được cập nhật thành công.";
            }

            WeakReferenceMessenger.Default.Send(new ValueChangedMessage<string>("close"));
        }
        catch (Exception ex)
        {
            SetStatusMessage($"Lỗi khi cập nhật: {ex.Message}", isError: true);
            ErrorMessage = $"Lỗi khi cập nhật: {ex.Message}";
            SuccessMessage = string.Empty;
        }
    }

    [RelayCommand]
    private void AddMaterial() => SelectedMaterials.Add(new ProductMaterial { Id = FullMaterials.FirstOrDefault()?.Id ?? Guid.Empty });

    [RelayCommand]
    private void DeleteMaterial(Guid id) => SelectedMaterials.Remove(SelectedMaterials.FirstOrDefault(m => m.Id == id)!);

    [RelayCommand]
    private async Task DeleteCategoryAsync(Guid id)
    {
        try
        {
            await _repository.DeleteCategory(id);

            var category = Categories.FirstOrDefault(c => c.Id == id);
            if (category == null) return;

            // Remove the category from the categories collection
            Categories.Remove(category);

            // Remove from selected categories if it's there
            var selectedCategory = SelectedCategories.FirstOrDefault(c => c.Id == id);
            if (selectedCategory != null)
                SelectedCategories.Remove(selectedCategory);

            // Remove products of deleted category from full products list
            _fullProducts.RemoveAll(p => p.CategoryId == id);

            // Update filtered products based on remaining selected categories
            FilterByMultipleCategories(SelectedCategories);
        }
        catch (Exception ex)
        {
            SetStatusMessage($"Error deleting category: {ex.Message}", isError: true);
        }
    }

    [RelayCommand]
    private async Task DeleteProductAsync(Guid id)
    {
        try
        {
            await _repository.DeleteProduct(id);
            var product = _fullProducts.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                _fullProducts.Remove(product);
                // Update filtered products based on current category selection
                FilterByMultipleCategories(SelectedCategories);
            }
        }
        catch (Exception ex)
        {
            SetStatusMessage($"Error deleting product: {ex.Message}", isError: true);
        }
    }

    [RelayCommand]
    private void LoadAllProducts()
    {
        FilteredProducts.Clear();
        foreach (var product in _fullProducts)
            FilteredProducts.Add(product);
    }

    [RelayCommand]
    private void FilterByMultipleCategories(IEnumerable<object> selectedItems)
    {
        if (selectedItems is ObservableCollection<Category> categories)
        {
            // If this is our own collection, use it directly
            FilterByCategories(categories);
            return;
        }

        // Otherwise, handle items coming from the UI control
        SelectedCategories.Clear();

        // If no categories are selected, show all products
        if (selectedItems == null || !selectedItems.Any())
        {
            LoadAllProducts();
            return;
        }

        // Add selected categories to our collection
        foreach (var item in selectedItems)
        {
            if (item is Category category)
                SelectedCategories.Add(category);
        }

        // Filter products based on selected categories
        FilterByCategories(SelectedCategories);
    }

    // New helper method for filtering by categories
    private void FilterByCategories(IEnumerable<Category> categories)
    {
        FilteredProducts.Clear();

        if (!categories.Any())
        {
            LoadAllProducts();
            return;
        }

        var categoryIds = categories.Select(c => c.Id).ToList();
        var filteredProducts = _fullProducts.Where(p => categoryIds.Contains(p.CategoryId)).ToList();

        foreach (var product in filteredProducts)
            FilteredProducts.Add(product);
    }
    #endregion

    #region Helper Methods
    private bool ValidateInput()
    {
        if (CurrentMode == Mode.AddingCategory || CurrentMode == Mode.EditingCategory)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                SetStatusMessage("Category name cannot be empty.", isError: true);
                return false;
            }
        }
        else if (CurrentMode == Mode.AddingProduct || CurrentMode == Mode.EditingProduct)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                SetStatusMessage("Product name cannot be empty.", isError: true);
                return false;
            }
            if (SelectedCategoryForEdit is null)
            {
                SetStatusMessage("Please select a category.", isError: true);
                return false;
            }
            if (Price <= 0)
            {
                SetStatusMessage("Price must be greater than 0.", isError: true);
                return false;
            }
            if (SelectedMaterials.Any(m => m.Id == Guid.Empty || m.Quantity <= 0))
            {
                SetStatusMessage("All materials must have valid IDs and quantities.", isError: true);
                return false;
            }
        }
        if (SelectedImage == null && (CurrentMode == Mode.AddingCategory || CurrentMode == Mode.AddingProduct))
        {
            SetStatusMessage("Please select an image.", isError: true);
            return false;
        }
        return true;
    }

    private void UpdateCategoryInCollections()
    {
        var index = Categories.IndexOf(Categories.FirstOrDefault(c => c.Id == _selectedItemId)!);
        if (index == -1) return;

        Categories[index] = new Category
        {
            Id = _selectedItemId,
            Name = Name,
            Image = _selectedFile?.Path ?? Categories[index].Image,
            CreatedAt = Categories[index].CreatedAt,
            UpdatedAt = DateTime.Now
        };
    }

    private void UpdateProductInCollections()
    {
        var index = _fullProducts.FindIndex(p => p.Id == _selectedItemId);
        if (index == -1) return;

        _fullProducts[index] = new Product
        {
            Id = _selectedItemId,
            Name = Name,
            CategoryId = SelectedCategoryForEdit!.Id,
            Price = Price,
            Image = _selectedFile?.Path ?? _fullProducts[index].Image,
            Materials = [.. SelectedMaterials],
            CreatedAt = _fullProducts[index].CreatedAt,
            UpdatedAt = DateTime.Now
        };
    }

    [RelayCommand]
    private void ResetForm()
    {
        Name = string.Empty;
        SelectedCategoryForEdit = null;
        Price = 0;

        SelectedMaterials.Clear();
        if (FullMaterials.Count != 0)
            SelectedMaterials.Add(new ProductMaterial { Id = FullMaterials[0].Id });

        _selectedFile = null;
        SelectedImage = null;
        _selectedItemId = Guid.Empty;

        CurrentMode = Mode.None;
        SetStatusMessage(string.Empty, isError: false);
    }

    private void SetStatusMessage(string message, bool isError)
    {
        if (isError)
        {
            ErrorMessage = message;
            SuccessMessage = string.Empty;
        }
        else
        {
            SuccessMessage = message;
            ErrorMessage = string.Empty;
        }
    }
    #endregion

    public void Receive(PropertyChangedMessage<object> message)
    {
        if (message.Sender.GetType() == typeof(InventoryViewModel) &&
            message.PropertyName == nameof(InventoryViewModel.Inventories))
        {
            FullMaterials = (List<Inventory>)message.NewValue;
            OnPropertyChanged(nameof(FullMaterials));
        }
    }
}