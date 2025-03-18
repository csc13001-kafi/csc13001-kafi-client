using System.Threading.Tasks;
using kafi.Models.Authentication;

namespace kafi.Contracts.Services
{
    public interface IAuthService
    {
        Task<LoginResponse?> LoginAsync(LoginRequest request);
    }
}
