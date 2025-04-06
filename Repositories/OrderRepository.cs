using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using kafi.Contracts;
using kafi.Data;
using kafi.Models;

namespace kafi.Repositories;

public interface IOrderRepository : IRepository<Order>
{
}

public class OrderRepository(IOrderDao dao) : IOrderRepository
{
    private readonly IOrderDao _orderDao = dao;

    public async Task<IEnumerable<Order>> GetAll()
    {
        return await _orderDao.GetAll();
    }

    public async Task<Order>? GetById(Guid id)
    {
        return await _orderDao.GetById(id)!;
    }

    public Task Delete(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<object> Add(object entity)
    {
        throw new NotImplementedException();
    }

    public Task Update(Guid id, object entity)
    {
        throw new NotImplementedException();
    }
}
