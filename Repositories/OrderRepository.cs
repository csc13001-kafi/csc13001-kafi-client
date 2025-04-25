using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using kafi.Contracts;
using kafi.Data;
using kafi.Models;

namespace kafi.Repositories;

public interface IOrderRepository : IRepository<Order>
{
    Task<PaymentStatus> GetPaymentStatus(int orderCode);
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
        if (entity is not CreateOrderRequest request)
        {
            throw new ArgumentException("Invalid entity type");
        }
        using var form = new MultipartFormDataContent();
        form.Add(new StringContent(request.Id.ToString()), "id");
        form.Add(new StringContent(request.Table), "table");
        form.Add(new StringContent(request.CreatedAt.ToString("O")), "time");
        form.Add(new StringContent(request.PaymentMethod), "paymentMethod");
        form.Add(new StringContent(request.ClientPhoneNumber), "clientPhoneNumber");

        var productIds = string.Join(",", request.Products.Select(p => p.ToString()));
        var quantities = string.Join(",", request.Quantities.Select(q => q.ToString()));

        form.Add(new StringContent(productIds), "products");
        form.Add(new StringContent(quantities), "quantities");
        return _orderDao.Add(form);
    }

    public Task<PaymentStatus> GetPaymentStatus(int orderCode)
    {
        return _orderDao.GetPaymentStatus(orderCode);
    }

    public Task Update(Guid id, object entity)
    {
        throw new NotImplementedException();
    }
}
