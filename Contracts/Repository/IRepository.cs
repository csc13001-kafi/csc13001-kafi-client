using System.Collections.Generic;
using System.Threading.Tasks;

namespace kafi.Contracts.Repository
{
    public interface IRepository<T>
        where T : class
    {
        Task<IEnumerable<T>> GetAll();
        Task Add(object entity);
        Task Update(string id, object entity);
        Task Delete(string id);
        Task<T>? GetById(string id);
    }
}
