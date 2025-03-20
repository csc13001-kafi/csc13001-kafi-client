using System.Collections.Generic;
using System.Threading.Tasks;

namespace kafi.Contracts.Repository
{
    public interface IRepository<T>
        where T : class
    {
        Task<IEnumerable<T>> GetAll();
        Task Add(T entity);
        Task Update(T entity);
        Task Delete(T entity);
        Task<T>? GetById(int id);
    }
}
