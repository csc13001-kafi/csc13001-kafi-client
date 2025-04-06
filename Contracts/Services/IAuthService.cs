using System.Threading.Tasks;
using kafi.Models;

namespace kafi.Contracts.Services;

public interface IAuthService
{
    bool IsInRole(Role role);
    User? CurrentUser { get; }
    Task LoadCurrentUserFromToken();
    Task<LoginResponse?> LoginAsync(LoginRequest request);
    Task<string> LogoutAsync();
    Task<string> ChangePasswordAsync(string oldPassword, string newPassword, string confirmPassword);
    Task<string> RequestForgotPasswordOtpAsync(string email);
    Task<string> VerifyOtpAsync(string email, string otp);
    Task<string> ResetPasswordAsync(string email, string otp, string newPassword, string confirmPassword);
}
