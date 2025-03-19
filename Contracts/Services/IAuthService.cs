using System.Threading.Tasks;
using kafi.Models;
using kafi.Models.Authentication;

namespace kafi.Contracts.Services
{
    public interface IAuthService
    {
        bool IsInRole(Role role);
        User? CurrentUser { get; }
        void LoadCurrentUserFromToken(string accessToken);
        Task<LoginResponse?> LoginAsync(LoginRequest request);
        Task<string> LogoutAsync();
    }
}
