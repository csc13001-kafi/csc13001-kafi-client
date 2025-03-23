using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using kafi.Contracts.Repository;
using kafi.Data;
using kafi.Models;

namespace kafi.Repositories
{
    public interface IInventoryRepository : IRepository<Inventory>
    {
    }

    public class InventoryRepository : IInventoryRepository
    {
        private readonly IInventoryDao _inventoryDao;

        public InventoryRepository(IInventoryDao inventoryDao)
        {
            _inventoryDao = inventoryDao;
        }

        public async Task<IEnumerable<Inventory>> GetAll()
        {
            var tmp = await _inventoryDao.GetAll();
            foreach (var item in tmp)
            {
                System.Diagnostics.Trace.WriteLine(item.Name);
                System.Diagnostics.Trace.WriteLine(item.OriginalStock);
                System.Diagnostics.Trace.WriteLine(item.CurrentStock);
                System.Diagnostics.Trace.WriteLine(item.Unit);
            }
            return tmp;
        }

        public async Task<Inventory>? GetById(string id)
        {
            throw new NotImplementedException();
        }

        public async Task Add(object inventory)
        {
            throw new NotImplementedException();
        }

        public async Task Update(string id, object inventory)
        {
            throw new NotImplementedException();
        }

        public async Task Delete(string id)
        {
            throw new NotImplementedException();
        }
    }
}
