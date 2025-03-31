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
using kafi.Data;
using kafi.Models;
using kafi.Models.Inventory;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage;

namespace kafi.ViewModels
{
    public partial class MenuViewModel : ObservableRecipient, IRecipient<PropertyChangedMessage<object>>

    {
        private readonly IMenuRepository _repository;
        private readonly IAuthService _authService;
        private readonly IWindowService _windowService;
        public MenuViewModel(IMenuRepository repository, IAuthService authService, IWindowService windowService)
        {
            _repository = repository;
            _authService = authService;
            _windowService = windowService;

            IsActive = true;
        }


        private Window Window => _windowService.GetCurrentWindow();
        public bool IsEmployee => _authService.IsInRole(Role.Employee);

        private List<Product> _fullProducts = [];
        private List<Category> _fullCategories = [];

        public List<Inventory> FullMaterials { get; set; } = [];
        public ObservableCollection<Category> Categories { get; set; } = [];
        public ObservableCollection<Product> FilteredProducts { get; set; } = [];
        public ObservableCollection<ProductMaterial> Materials { get; set; } = [];

        [ObservableProperty]
        private Category selectedCategory;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsProduct))]
        private bool isAddingProduct;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(TurnOnEditingProductCommand))]
        [NotifyPropertyChangedFor(nameof(IsProduct))]
        [NotifyPropertyChangedFor(nameof(IsEditing))]
        private bool isEditingProduct;

        public bool IsProduct => IsAddingProduct || IsEditingProduct;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsCategory))]
        private bool isAddingCategory;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(TurnOnEditingCategoryCommand))]
        [NotifyPropertyChangedFor(nameof(IsCategory))]
        [NotifyPropertyChangedFor(nameof(IsEditing))]
        private bool isEditingCategory;

        public bool IsCategory => IsAddingCategory || IsEditingCategory;

        public bool IsEditing => IsEditingCategory || IsEditingProduct;

        [ObservableProperty]
        private bool isPickerEnable = true;

        private StorageFile? _selectedCategoryFile = null;
        private StorageFile? _selectedProductFile = null;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsImageExist))]
        private ImageSource? selectedImage = null;

        public bool IsImageExist => SelectedImage != null;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddCommand))]
        private string categoryName;

        public Guid SelectedCategoryId = Guid.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddCommand))]
        private string productName;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddCommand))]
        private Guid productCategoryId = Guid.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddCommand))]
        private int productPrice;

        public Guid SelectedProductId = Guid.Empty;

        [ObservableProperty]
        private string statusMessage;

        private bool CanLoadData() => _fullCategories.Count == 0 || _fullProducts.Count == 0;
        [RelayCommand(CanExecute = nameof(CanLoadData))]
        private async Task LoadData()
        {
            IsLoading = true;

            try
            {
                var response = await _repository.GetCategoriesAndProducts();
                _fullProducts = response.Products;
                _fullCategories = response.Categories;

                if (_fullCategories.Count != 0)
                {
                    LoadCategoriesAndProductToView(_fullCategories[0]);
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading data: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private bool CanLoadMaterials() => FullMaterials.Count == 0;
        [RelayCommand(CanExecute = nameof(CanLoadMaterials))]
        private async Task LoadMaterials()
        {
            try
            {
                FullMaterials = [.. await _repository.GetMaterials()];
                Materials.Clear();
                Materials.Add(new ProductMaterial
                {
                    Id = FullMaterials.FirstOrDefault().Id,
                });
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading materials: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task PickImageAsync()
        {
            IsPickerEnable = false;
            try
            {
                var picker = new Windows.Storage.Pickers.FileOpenPicker();
                var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(Window);

                WinRT.Interop.InitializeWithWindow.Initialize(picker, hWnd);

                picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
                picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
                picker.FileTypeFilter.Add(".jpg");
                picker.FileTypeFilter.Add(".jpeg");
                picker.FileTypeFilter.Add(".png");
                var file = await picker.PickSingleFileAsync();

                if (file == null)
                {
                    return;
                }


                var stream = await file.OpenAsync(FileAccessMode.Read);
                var bitmapImage = new BitmapImage();
                await bitmapImage.SetSourceAsync(stream);
                SelectedImage = bitmapImage;


                if (IsCategory)
                {
                    _selectedCategoryFile = file;
                }
                if (IsProduct)
                {
                    _selectedProductFile = file;
                }
                AddCommand.NotifyCanExecuteChanged();
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error picking image: {ex.Message}");
            }
            finally
            {
                IsPickerEnable = true;
            }
        }

        private bool CanAddCategory() => IsAddingCategory && !string.IsNullOrEmpty(CategoryName) && _selectedCategoryFile != null;
        private async Task AddCategoryAsync()
        {
            IsPickerEnable = false;
            try
            {
                var request = new CreateCategoryRequest
                (
                    Name: CategoryName,
                    FileStream: await _selectedCategoryFile.OpenStreamForReadAsync(),
                    ContentType: _selectedCategoryFile.ContentType,
                    FileName: _selectedCategoryFile.Name
                );

                var newCategory = await _repository.AddCategory(request);

                _fullCategories.Add(newCategory);
                Categories.Add(newCategory);
                DeleteAllInput();
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding category: {ex.Message}");
            }
            finally
            {
                IsPickerEnable = true;
            }
        }
        private bool CanAddProduct() => IsAddingProduct && !string.IsNullOrEmpty(ProductName) && ProductCategoryId != Guid.Empty && _selectedProductFile != null;
        private async Task AddProductAsync()
        {
            IsPickerEnable = false;
            try
            {
                var request = new CreateProductRequest
                (
                    Name: ProductName,
                    CategoryId: ProductCategoryId,
                    Price: ProductPrice,
                    IsAvailable: true,
                    FileStream: await _selectedProductFile.OpenStreamForReadAsync(),
                    ContentType: _selectedProductFile.ContentType,
                    FileName: _selectedProductFile.Name,
                    Materials: [.. Materials]
                );

                var newProduct = await _repository.AddProduct(request);
                _fullProducts.Add(newProduct);
                FilterByCategory(SelectedCategory);
                DeleteAllInput();
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding product: {ex.Message}");
            }
            finally
            {
                IsPickerEnable = true;
            }
        }

        private bool CanAdd()
        {
            if (IsCategory)
            {
                return CanAddCategory();
            }
            else if (IsProduct)
            {
                return CanAddProduct();
            }
            return false;
        }
        [RelayCommand(CanExecute = nameof(CanAdd))]
        private async Task AddAsync()
        {
            if (IsCategory)
            {
                await AddCategoryAsync();
            }
            else if (IsProduct)
            {
                await AddProductAsync();
            }
        }

        [RelayCommand]
        private async Task TurnOnAddingProductAsync()
        {
            IsAddingProduct = true;
            if (_selectedProductFile != null)
            {
                var stream = await _selectedProductFile.OpenAsync(FileAccessMode.Read);
                var bitmapImage = new BitmapImage();
                await bitmapImage.SetSourceAsync(stream);
                SelectedImage = bitmapImage;
            }
            else
                SelectedImage = null;
            AddCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand]
        private async Task TurnOnAddingCategoryAsync()
        {
            IsAddingCategory = true;
            if (_selectedCategoryFile != null)
            {
                var stream = await _selectedCategoryFile.OpenAsync(FileAccessMode.Read);
                var bitmapImage = new BitmapImage();
                await bitmapImage.SetSourceAsync(stream);
                SelectedImage = bitmapImage;
            }
            else
                SelectedImage = null;
            AddCommand.NotifyCanExecuteChanged();
        }

        private bool CanTurnOnEditingProduct(Guid id) => !IsEditingProduct && id != Guid.Empty;
        [RelayCommand(CanExecute = nameof(CanTurnOnEditingProduct))]
        private void TurnOnEditingProduct(Guid id)
        {
            var editingProduct = _fullProducts.FirstOrDefault(p => p.Id == id);
            if (editingProduct == null)
            {
                return;
            }

            ProductName = editingProduct.Name;
            ProductCategoryId = editingProduct.CategoryId;
            ProductPrice = editingProduct.Price;
            Materials.Clear();
            if (editingProduct.Materials.Length > 0)
            {
                foreach (var material in editingProduct.Materials)
                {
                    Materials.Add(new ProductMaterial
                    {
                        Id = material.Id,
                        Quantity = material.Quantity
                    });
                }
            }

            SelectedImage = new BitmapImage(new Uri(editingProduct.Image));
            SelectedProductId = editingProduct.Id;
            IsEditingProduct = true;
            UpdateCommand.NotifyCanExecuteChanged();
        }

        private bool CanUpdateProduct() => IsEditingProduct && SelectedProductId != Guid.Empty;
        private async Task UpdateProductAsync()
        {
            try
            {
                if (_selectedProductFile == null)
                {
                    var request = new CreateProductRequest
                    (
                        Name: ProductName,
                        CategoryId: ProductCategoryId,
                        Price: ProductPrice,
                        IsAvailable: true,
                        FileStream: null,
                        ContentType: null,
                        FileName: null,
                        Materials: [.. Materials]
                    );
                    await _repository.UpdateProduct(SelectedProductId, request);
                }
                else
                {
                    var request = new CreateProductRequest
                    (
                        Name: ProductName,
                        CategoryId: ProductCategoryId,
                        Price: ProductPrice,
                        IsAvailable: true,
                        FileStream: await _selectedProductFile.OpenStreamForReadAsync(),
                        ContentType: _selectedProductFile.ContentType,
                        FileName: _selectedProductFile.Name,
                        Materials: [.. Materials]
                    );
                    await _repository.UpdateProduct(SelectedProductId, request);
                }
                var index = _fullProducts.FindIndex(p => p.Id == SelectedProductId);
                _fullProducts[index] = new Product
                {
                    Id = SelectedProductId,
                    Name = ProductName,
                    CategoryId = ProductCategoryId,
                    Price = ProductPrice,
                    Image = _selectedProductFile == null ? _fullProducts[index].Image : _selectedProductFile.Path,
                    CreatedAt = _fullProducts[index].CreatedAt,
                    UpdatedAt = DateTime.Now
                };
                FilterByCategory(SelectedCategory);
                DeleteAllInput();
                WeakReferenceMessenger.Default.Send(new ValueChangedMessage<string>(StatusMessage));
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating product: {ex.Message}");
            }
        }

        private bool CanTurnOnEditingCategory(Guid id) => !IsEditingCategory && id != Guid.Empty;
        [RelayCommand(CanExecute = nameof(CanTurnOnEditingCategory))]
        private void TurnOnEditingCategory(Guid id)
        {
            var category = _fullCategories.FirstOrDefault(c => c.Id == id);
            if (category == null)
            {
                return;
            }

            CategoryName = category.Name;
            SelectedImage = new BitmapImage(new Uri(category.Image));

            SelectedCategoryId = category.Id;
            IsEditingCategory = true;
            UpdateCommand.NotifyCanExecuteChanged();
        }

        private bool CanUpdateCategory() => IsEditingCategory && SelectedCategoryId != Guid.Empty;
        private async Task UpdateCategoryAsync()
        {
            try
            {
                if (_selectedCategoryFile == null)
                {
                    var request = new CreateCategoryRequest(Name: CategoryName, FileStream: null, ContentType: null, FileName: null);
                    await _repository.UpdateCategory(SelectedCategoryId, request);
                }
                else
                {
                    var request = new CreateCategoryRequest
                    (
                        Name: CategoryName,
                        FileStream: await _selectedCategoryFile.OpenStreamForReadAsync(),
                        ContentType: _selectedCategoryFile.ContentType,
                        FileName: _selectedCategoryFile.Name
                    );
                    await _repository.UpdateCategory(SelectedCategoryId, request);
                }

                var index = _fullCategories.FindIndex(c => c.Id == SelectedCategoryId);
                _fullCategories[index] = new Category
                {
                    Id = SelectedCategoryId,
                    Name = CategoryName,
                    Image = _selectedCategoryFile == null ? _fullCategories[index].Image : _selectedCategoryFile.Path,
                    CreatedAt = _fullCategories[index].CreatedAt,
                    UpdatedAt = DateTime.Now
                };

                LoadCategoriesAndProductToView(_fullCategories[index]);
                DeleteAllInput();
                WeakReferenceMessenger.Default.Send(new ValueChangedMessage<string>(StatusMessage));
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating category: {ex.Message}");
            }
        }

        private bool CanUpdate()
        {
            if (IsCategory)
            {
                return CanUpdateCategory();
            }
            else if (IsProduct)
            {
                return CanUpdateProduct();
            }
            return false;
        }
        [RelayCommand(CanExecute = nameof(CanUpdate))]
        private async Task UpdateAsync()
        {
            if (IsCategory)
            {
                await UpdateCategoryAsync();
            }
            else if (IsProduct)
            {
                await UpdateProductAsync();
            }
        }

        [RelayCommand]
        private void AddMaterial() => Materials.Add(new ProductMaterial
        {
            Id = FullMaterials.FirstOrDefault().Id,
        });

        [RelayCommand]
        private void DeleteMaterial(Guid Id) => Materials.Remove(Materials.FirstOrDefault(m => m.Id == Id));

        [RelayCommand]
        private void DeleteAllInput()
        {
            if (IsAddingProduct || IsEditingProduct)
            {
                ProductName = string.Empty;
                ProductCategoryId = Categories.FirstOrDefault().Id;
                ProductPrice = 0;
                Materials.Clear();
                Materials.Add(new ProductMaterial
                {
                    Id = FullMaterials.FirstOrDefault().Id,
                });
                _selectedProductFile = null;
                SelectedImage = null;
            }
            else if (IsAddingCategory || IsEditingCategory)
            {
                CategoryName = string.Empty;
                _selectedCategoryFile = null;
                SelectedImage = null;
            }
        }

        private void LoadCategoriesAndProductToView(Category selectedCategory)
        {
            Categories.Clear();
            foreach (var category in _fullCategories)
            {
                Categories.Add(category);
            }
            ProductCategoryId = Categories.FirstOrDefault().Id;
            FilterByCategory(selectedCategory);
        }

        [RelayCommand]
        private void FilterByCategory(Category category)
        {
            if (category == null) return;

            var filtered = _fullProducts.Where(p => p.CategoryId == category.Id);
            FilteredProducts.Clear();
            foreach (var product in filtered)
            {
                FilteredProducts.Add(product);
            }
            SelectedCategory = category;
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
                    FilterByCategory(SelectedCategory);
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting product: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task DeleteCategoryAsync(Guid id)
        {
            try
            {
                await _repository.DeleteCategory(id);
                var category = _fullCategories.FirstOrDefault(c => c.Id == id);
                if (category != null)
                {
                    _fullCategories.Remove(category);
                    Categories.Remove(category);
                    if (SelectedCategory.Id == id)
                    {
                        SelectedCategory = Categories.FirstOrDefault();
                    }
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting category: {ex.Message}");
            }
        }

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
}
