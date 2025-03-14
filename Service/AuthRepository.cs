using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using kafi.Contracts;
using kafi.Models.Authentication;
using Newtonsoft.Json;

namespace kafi.Service
{
    public class AuthRepository : IAuthRepository
    {
        private readonly HttpClient _httpClient;
        public AuthRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<LoginResponse?> LoginAsync(LoginRequest request)
        {
            var jsonPayload = JsonConvert.SerializeObject(request);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("auth/sign-in", content);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(responseContent);
                return loginResponse;
            }
            else
            {
                return null;
            }
        }
    }
}
