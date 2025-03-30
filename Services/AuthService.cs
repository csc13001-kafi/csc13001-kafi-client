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
    public class AuthService(IHttpClientFactory httpClientFactory) : IAuthService
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("Common");

        public User? CurrentUser { get; private set; }

        public bool IsInRole(Role role)
        {
            return CurrentUser != null && CurrentUser.Role == role;
        }

        public async Task LoadCurrentUserFromToken(string accessToken)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                var response = await _httpClient.GetAsync("/users/user");
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
                var response = await _httpClient.PostAsync("auth/sign-in", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseContent);

                    await LoadCurrentUserFromToken(loginResponse.AccessToken);

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
                var response = await _httpClient.DeleteAsync("auth/sign-out");
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var logoutResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent);
                    var message = logoutResponse!["message"].ToString();
                    CurrentUser = null;
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
                var response = await _httpClient.PutAsync("auth/change-password", content);
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
    }
}
