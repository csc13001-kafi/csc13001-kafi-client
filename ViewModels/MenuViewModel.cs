using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using kafi.Models;
using kafi.Data;

namespace kafi.ViewModels
{
    public partial class MenuViewModel : ObservableObject
    {
        private readonly IMenuRepository _menuRepository;

        [ObservableProperty]
        private ObservableCollection<Category> categories;

        [ObservableProperty]
        private ObservableCollection<Product> allProducts;

        [ObservableProperty]
        private ObservableCollection<Product> filteredProducts;

        [ObservableProperty]
        private Category selectedCategory;

        [ObservableProperty]
        private bool isLoading;

        public MenuViewModel(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
            categories = new ObservableCollection<Category>();
            allProducts = new ObservableCollection<Product>();
            filteredProducts = new ObservableCollection<Product>();
        }

        [RelayCommand]
        private async Task LoadData()
        {
            IsLoading = true;

            try
            {
                var response = await _menuRepository.GetCategoriesAndProducts();
                var categoriesList = response.Categories.ToList();
                var productsList = response.Products.ToList();

                Categories = new ObservableCollection<Category>(categoriesList);
                AllProducts = new ObservableCollection<Product>(productsList);
                if (Categories.Count > 0)
                {
                    FilterByCategory(Categories[0]);
                }
                else
                {
                    FilteredProducts = new ObservableCollection<Product>(productsList);
                }            
            }
            catch (System.Exception ex)
            {
                // Handle error
                System.Diagnostics.Debug.WriteLine($"Error loading data: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private void FilterByCategory(Category category)
        {
            if (category == null) return;

            if (category.Id == "0") // "All Products" category
            {
                FilteredProducts = new ObservableCollection<Product>(AllProducts);
            }
            else
            {
                var filtered = AllProducts.Where(p => p.CategoryId == category.Id);
                FilteredProducts = new ObservableCollection<Product>(filtered);
            }

            SelectedCategory = category;
        }
    }
}
