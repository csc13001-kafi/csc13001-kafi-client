using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using kafi.Contracts.Services;
using kafi.Models;
using kafi.Models.Authentication;

namespace kafi.Service
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _authHttpClient;
        private readonly HttpClient _noAuthHttpClient;
        private readonly ISecureTokenStorage _secureTokenStorage;

        public AuthService(IHttpClientFactory httpClientFactory, ISecureTokenStorage secureTokenStorage)
        {
            _authHttpClient = httpClientFactory.CreateClient("Common");
            _secureTokenStorage = secureTokenStorage;
            _noAuthHttpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:8080/")
            };
        }

        public User? CurrentUser { get; private set; }

        public bool IsInRole(Role role)
        {
            return CurrentUser != null && CurrentUser.Role == role;
        }

        public async Task LoadCurrentUserFromToken()
        {
            try
            {
                var response = await _authHttpClient.GetAsync("/users/user");
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    JsonSerializerOptions options = new()
                    {
                        Converters = { new JsonStringEnumConverter() }
                    };
                    var user = JsonSerializer.Deserialize<User>(responseContent, options);
                    CurrentUser = user;
                }
                else
                {
                    Debug.WriteLine($"Load current user failed. Status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception during load current user: {ex}");
            }
        }

        public async Task<LoginResponse?> LoginAsync(LoginRequest request)
        {
            try
            {
                var jsonPayload = JsonSerializer.Serialize(request);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                var response = await _noAuthHttpClient.PostAsync("auth/sign-in", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseContent);

                    _secureTokenStorage.SaveTokens(loginResponse.AccessToken, loginResponse.RefreshToken);
                    await LoadCurrentUserFromToken();

                    return loginResponse;
                }
                Debug.WriteLine($"Login failed. Status code: {response.StatusCode}");
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception during login: {ex}");
                return null;
            }
        }

        public async Task<string> LogoutAsync()
        {
            try
            {
                var response = await _authHttpClient.DeleteAsync("auth/sign-out");
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var logoutResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent);
                    var message = logoutResponse!["message"].ToString();
                    CurrentUser = null;
                    _secureTokenStorage.ClearTokens();
                    return message;
                }
                Debug.WriteLine($"Logout failed. Status code: {response.StatusCode}");
                return "An error occurred while logging out.";
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception during logout: {ex}");
                return "An error occurred while logging out.";
            }
        }

        public async Task<string> ChangePasswordAsync(string oldPassword, string newPassword, string confirmPassword)
        {
            try
            {
                var payload = new
                {
                    oldPassword,
                    newPassword,
                    confirmPassword
                };
                var jsonPayload = JsonSerializer.Serialize(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                var response = await _authHttpClient.PutAsync("auth/change-password", content);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var changePasswordResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent);
                    var message = changePasswordResponse!["message"].ToString();
                    return message;
                }
                Debug.WriteLine($"Change password failed. Status code: {response.StatusCode}");
                return "An error occurred while changing the password.";
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception during change password: {ex}");
                return "An error occurred while changing the password.";
            }
        }

        public async Task<string> RequestForgotPasswordOtpAsync(string email)
        {
            try
            {
                var payload = new
                {
                    email
                };
                var jsonPayload = JsonSerializer.Serialize(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                var response = await _noAuthHttpClient.PostAsync("auth/password-recovery", content);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var forgotPasswordResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent);
                    var message = forgotPasswordResponse!["message"].ToString();
                    return message;
                }
                Debug.WriteLine($"Request forgot password OTP failed. Status code: {response.StatusCode}");
                return "An error occurred while requesting the OTP.";
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception during request forgot password OTP: {ex}");
                return "An error occurred while requesting the OTP.";
            }
        }

        public async Task<string> VerifyOtpAsync(string email, string otp)
        {
            try
            {
                var payload = new
                {
                    email,
                    otp
                };
                var jsonPayload = JsonSerializer.Serialize(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                var response = await _noAuthHttpClient.PostAsync("auth/verify-otp", content);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var verifyOtpResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent);
                    var message = verifyOtpResponse!["message"].ToString();
                    return message;
                }
                Debug.WriteLine($"Verify OTP failed. Status code: {response.StatusCode}");
                return "An error occurred while verifying the OTP.";
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception during verify OTP: {ex}");
                return "An error occurred while verifying the OTP.";
            }
        }

        public async Task<string> ResetPasswordAsync(string email, string otp, string newPassword, string confirmPassword)
        {
            try
            {
                var payload = new
                {
                    email,
                    otp,
                    newPassword,
                    confirmPassword
                };
                var jsonPayload = JsonSerializer.Serialize(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                var response = await _noAuthHttpClient.PostAsync("auth/reset-password", content);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var resetPasswordResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent);
                    var message = resetPasswordResponse!["message"].ToString();
                    return message;
                }
                Debug.WriteLine($"Reset password failed. Status code: {response.StatusCode}");
                return "An error occurred while resetting the password.";
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception during reset password: {ex}");
                return "An error occurred while resetting the password.";
            }
        }
    }
}
