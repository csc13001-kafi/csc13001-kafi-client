using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using kafi.Contracts;
using kafi.Data;
using kafi.Models;

namespace kafi.Repositories;

public interface IInventoryRepository : IRepository<Inventory>
{
}

public class InventoryRepository(IInventoryDao inventoryDao) : IInventoryRepository
{
    private readonly IInventoryDao _inventoryDao = inventoryDao;

    public async Task<IEnumerable<Inventory>> GetAll()
    {
        return await _inventoryDao.GetAll();
    }

    public async Task<Inventory>? GetById(Guid id)
    {
        return await _inventoryDao.GetById(id)!;
    }

    public async Task<object> Add(object inventory)
    {
        return await _inventoryDao.Add(inventory);
    }

    public async Task Update(Guid id, object inventory)
    {
        await _inventoryDao.Update(id, inventory);
    }

    public async Task Delete(Guid id)
    {
        await _inventoryDao.Delete(id);
    }
}
