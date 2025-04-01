using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using kafi.Models;
using kafi.Models.Inventory;

namespace kafi.Data
{
    public interface IMenuRepository
    {
        Task<CategoryProductsResponse> GetCategoriesAndProducts();
        Task<IEnumerable<Inventory>> GetMaterials();

        Task<Product> AddProduct(CreateProductRequest request);
        Task UpdateProduct(Guid id, CreateProductRequest request);
        Task DeleteProduct(Guid id);

        Task<Category> AddCategory(CreateCategoryRequest request);
        Task UpdateCategory(Guid id, CreateCategoryRequest request);
        Task DeleteCategory(Guid id);
    }

    public class MenuRepository(IMenuDao menuDao, ICategoryDao categoryDao, IProductDao productDao, IInventoryDao inventoryDao) : IMenuRepository
    {
        private readonly IMenuDao _menuDao = menuDao;
        private readonly ICategoryDao _categoryDao = categoryDao;
        private readonly IProductDao _productDao = productDao;
        private readonly IInventoryDao _inventoryDao = inventoryDao;

        public async Task<Category> AddCategory(CreateCategoryRequest request)
        {
            using var form = new MultipartFormDataContent();
            var fileContent = new StreamContent(request.FileStream);
            fileContent.Headers.Add("Content-Type", request.ContentType);

            form.Add(fileContent, "file", request.FileName);
            form.Add(new StringContent(request.Name), "name");

            return (Category)await _categoryDao.Add(form);
        }

        public async Task<Product> AddProduct(CreateProductRequest request)
        {
            using var form = new MultipartFormDataContent();
            var fileContent = new StreamContent(request.FileStream);
            fileContent.Headers.Add("Content-Type", request.ContentType);

            form.Add(fileContent, "file", request.FileName);
            form.Add(new StringContent(request.Name), "name");
            form.Add(new StringContent(request.Price.ToString()), "price");
            form.Add(new StringContent(request.IsAvailable.ToString()), "onStock");
            form.Add(new StringContent(request.CategoryId.ToString()), "categoryId");

            var materialIds = string.Join(",", request.Materials.Select(m => m.Id.ToString()));
            var quantities = string.Join(",", request.Materials.Select(m => m.Quantity.ToString()));

            form.Add(new StringContent(materialIds), "materials");
            form.Add(new StringContent(quantities), "quantity");
            return (Product)await _productDao.Add(form);
        }

        public async Task DeleteCategory(Guid id)
        {
            await _categoryDao.Delete(id);
        }

        public async Task DeleteProduct(Guid id)
        {
            await _productDao.Delete(id);
        }

        public async Task<CategoryProductsResponse> GetCategoriesAndProducts()
        {
            var response = await _menuDao.GetCategoriesAndProducts();
            var products = await _productDao.GetAll();
            response.Products = [.. products];
            return response;
        }

        public async Task<IEnumerable<Inventory>> GetMaterials()
        {
            return await _inventoryDao.GetAll();
        }

        public async Task UpdateCategory(Guid id, CreateCategoryRequest request)
        {
            using var form = new MultipartFormDataContent();
            if (request.FileStream != null)
            {
                var fileContent = new StreamContent(request.FileStream);
                fileContent.Headers.Add("Content-Type", request.ContentType);
                form.Add(fileContent, "file", request.FileName);
            }
            form.Add(new StringContent(request.Name), "name");

            await _categoryDao.Update(id, form);
        }

        public async Task UpdateProduct(Guid id, CreateProductRequest request)
        {
            using var form = new MultipartFormDataContent();
            if (request.FileStream != null)
            {
                var fileContent = new StreamContent(request.FileStream);
                fileContent.Headers.Add("Content-Type", request.ContentType);
                form.Add(fileContent, "file", request.FileName);
            }
            form.Add(new StringContent(request.Name), "name");
            form.Add(new StringContent(request.Price.ToString()), "price");
            form.Add(new StringContent(request.IsAvailable.ToString()), "onStock");
            form.Add(new StringContent(request.CategoryId.ToString()), "categoryId");
            var materialIds = string.Join(",", request.Materials.Select(m => m.Id.ToString()));
            var quantities = string.Join(",", request.Materials.Select(m => m.Quantity.ToString()));
            form.Add(new StringContent(materialIds), "materials");
            form.Add(new StringContent(quantities), "quantity");
            await _productDao.Update(id, form);
        }
    }
}