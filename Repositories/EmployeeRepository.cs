using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using kafi.Contracts;
using kafi.Data;
using kafi.Models;

namespace kafi.Repositories;

public interface IEmployeeRepository : IRepository<User>
{
}

public class EmployeeRepository(IEmployeeDao dao) : IEmployeeRepository
{
    private readonly IEmployeeDao _dao = dao;
    public async Task<object> Add(object entity)
    {
        return await _dao.Add(entity);
    }

    public async Task Delete(Guid id)
    {
        await _dao.Delete(id);
    }

    public async Task<IEnumerable<User>> GetAll()
    {
        return await _dao.GetAll();
    }

    public Task<User>? GetById(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task Update(Guid id, object entity)
    {
        await _dao.Update(id, entity);
    }
}
