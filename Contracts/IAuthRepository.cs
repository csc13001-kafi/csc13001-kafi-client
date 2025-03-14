using System.Threading.Tasks;
using kafi.Models.Authentication;

namespace kafi.Contracts
{
    public interface IAuthRepository
    {
        Task<LoginResponse?> LoginAsync(LoginRequest request);
    }
}
