using kafi.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kafi.Data
{
    public interface IMenuRepository
    {
        Task<IEnumerable<Product>> GetAllMenuItems();
        Task<Product?> GetMenuItemById(string id);
        Task<IEnumerable<Category>> GetAllCategories();
        Task<CategoryProductsResponse> GetCategoriesAndProducts();
        Task<IEnumerable<Product>> GetMenuItemsByCategory(int categoryId);
    }

    public class MenuRepository : IMenuRepository
    {
        private readonly IMenuDao _menuDao;

        public MenuRepository(IMenuDao menuDao)
        {
            _menuDao = menuDao;
        }

        public async Task<CategoryProductsResponse> GetCategoriesAndProducts()
        {
            return await _menuDao.GetCategoriesAndProducts();
        }

        public async Task<IEnumerable<Product>> GetAllMenuItems()
        {
            return await _menuDao.GetAll();
        }

        public async Task<Product?> GetMenuItemById(string id)
        {
            return await _menuDao.GetById(id);
        }

        public async Task<IEnumerable<Category>> GetAllCategories()
        {
            var response = await _menuDao.GetCategoriesAndProducts();
            return response.Categories;
        }

        public async Task<IEnumerable<Product>> GetMenuItemsByCategory(int categoryId)
        {
            var response = await _menuDao.GetCategoriesAndProducts();
            return response.Products.Where(p => p.OptionGroups.Any(og => og.ProductId == categoryId));
        }
    }
}