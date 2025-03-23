using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using kafi.Contracts.Repository;
using kafi.Data;
using kafi.Models;

namespace kafi.Repositories
{
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

        public async Task<Order>? GetById(string id)
        {
            throw new NotImplementedException();
        }

        public async Task Delete(string id)
        {
            throw new NotImplementedException();
        }

        public Task Add(object entity)
        {
            throw new NotImplementedException();
        }

        public Task Update(string id, object entity)
        {
            throw new NotImplementedException();
        }
    }
}
