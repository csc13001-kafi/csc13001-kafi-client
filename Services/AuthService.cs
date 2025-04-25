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

namespace kafi.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _authHttpClient;
    private readonly HttpClient _noAuthHttpClient;
    private readonly ISecureTokenStorage _secureTokenStorage;

    public AuthService(IHttpClientFactory httpClientFactory, ISecureTokenStorage secureTokenStorage)
    {
        _authHttpClient = httpClientFactory.CreateClient("Common");
        _secureTokenStorage = secureTokenStorage;

        var configuration = App.Configuration;

        _noAuthHttpClient = new HttpClient
        {
            BaseAddress = new Uri(configuration["ApiSettings:BaseUrl"] ?? throw new InvalidOperationException("API base URL is not configured in appsettings.json")),
            DefaultRequestHeaders =
            {
                { "Accept", "application/json" }
            }
        };
    }

    public User? CurrentUser { get; private set; }

    public bool IsInRole(Role role)
    {
        return CurrentUser != null && CurrentUser.Role == role;
    }

    public async Task<bool> LoadCurrentUserFromToken()
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
                return true;
            }
            else
            {
                Debug.WriteLine($"Load current user failed. Status code: {response.StatusCode}");
                return false;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception during load current user: {ex}");
            return false;
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
                var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseContent)!;

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
        var response = await _authHttpClient.DeleteAsync("auth/sign-out");
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var logoutResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent);
        var message = logoutResponse!["message"].ToString() ?? string.Empty;
        CurrentUser = null;
        _secureTokenStorage.ClearTokens();
        return message;
    }

    public async Task<string> ChangePasswordAsync(string oldPassword, string newPassword, string confirmPassword)
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
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var changePasswordResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent);
        var message = changePasswordResponse!["message"].ToString() ?? string.Empty;
        return message;
    }

    public async Task<string> RequestForgotPasswordOtpAsync(string email)
    {
        var payload = new
        {
            email
        };
        var jsonPayload = JsonSerializer.Serialize(payload);
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
        var response = await _noAuthHttpClient.PostAsync("auth/password-recovery", content);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var changePasswordResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent);
        var message = changePasswordResponse!["message"].ToString() ?? string.Empty;
        return message;
    }

    public async Task<string> VerifyOtpAsync(string email, string otp)
    {
        var payload = new
        {
            email,
            otp
        };
        var jsonPayload = JsonSerializer.Serialize(payload);
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
        var response = await _noAuthHttpClient.PostAsync("auth/verify-otp", content);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var changePasswordResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent);
        var message = changePasswordResponse!["message"].ToString() ?? string.Empty;
        return message;
    }

    public async Task<string> ResetPasswordAsync(string email, string otp, string newPassword, string confirmPassword)
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
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var changePasswordResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent);
        var message = changePasswordResponse!["message"].ToString() ?? string.Empty;
        return message;
    }
}
