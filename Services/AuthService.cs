using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using kafi.Contracts.Services;
using kafi.Models;
using kafi.Models.Authentication;

namespace kafi.Service
{
    public class AuthService(HttpClient httpClient) : IAuthService
    {
        private readonly HttpClient _httpClient = httpClient;

        public User? CurrentUser { get; private set; }

        public bool IsInRole(Role role)
        {
            return CurrentUser != null && CurrentUser.Role == role;
        }

        public void LoadCurrentUserFromToken(string accessToken)
        {
            var (id, username, role) = getUserFromToken(accessToken);
            if (role != "Manager" && role != "Employee")
                throw new InvalidOperationException("Invalid role in token");
            CurrentUser = new User
            {
                Id = id,
                Name = username,
                Role = role == "Manager" ? Role.Manager : Role.Employee
            };
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

                    LoadCurrentUserFromToken(loginResponse.AccessToken);

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

        private (string, string, string) getUserFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var claims = jwtToken.Claims.ToDictionary(claim => claim.Type, claim => claim.Value);

            var id = claims.GetValueOrDefault("id", "");
            var username = claims.GetValueOrDefault("username", "");
            var role = claims.GetValueOrDefault("role", "");

            return (id, username, role);
        }

        public async Task<string> LogoutAsync()
        {
            try
            {
                var response = await _httpClient.PostAsync("auth/sign-out", null);
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
    }
}
