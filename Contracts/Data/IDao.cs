using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace kafi.Contracts.Data
{
    public interface IDao<T>
        where T : class
    {
        Task<IEnumerable<T>> GetAll();
        Task<object> Add(object entity);
        Task Update(Guid id, object entity);
        Task Delete(Guid id);
        Task<T>? GetById(Guid id);
    }
}
