using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using kafi.Contracts.Services;
using kafi.Models.Authentication;
using Newtonsoft.Json;

namespace kafi.Service
{
    public class AuthMessageHandler : DelegatingHandler
    {
        private readonly ISecureTokenStorage _tokenStorage;

        public AuthMessageHandler(ISecureTokenStorage secureTokenStorage)
        {
            _tokenStorage = secureTokenStorage;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var (accessToken, refreshToken) = _tokenStorage.GetTokens();

            if (!string.IsNullOrEmpty(accessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                if (!string.IsNullOrEmpty(refreshToken))
                {
                    bool refreshed = await TryRefreshTokenAsync(refreshToken);
                    if (refreshed)
                    {
                        var clonedRequest = await CloneHttpRequestMessageAsync(request);
                        (accessToken, _) = _tokenStorage.GetTokens();
                        clonedRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                        response = await base.SendAsync(clonedRequest, cancellationToken);
                    }

                }
            }
            return response;
        }

        private async Task<bool> TryRefreshTokenAsync(string refreshToken)
        {
            var refreshRequest = new RefreshTokenRequest { refreshToken = refreshToken };
            var jsonPayload = JsonConvert.SerializeObject(refreshRequest);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:8080/");
                var response = await client.PostAsync("auth/refresh-token", content);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var refreshResponse = JsonConvert.DeserializeObject<RefreshTokenResponse>(responseContent);
                    // Save new tokens securely.
                    _tokenStorage.SaveTokens(refreshResponse.accessToken, refreshResponse.refreshToken);
                    return true;
                }
            }
            return false;
        }

        private async Task<HttpRequestMessage> CloneHttpRequestMessageAsync(HttpRequestMessage request)
        {
            var clone = new HttpRequestMessage(request.Method, request.RequestUri);
            foreach (var header in request.Headers)
            {
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
            if (request.Content != null)
            {
                var contentBytes = await request.Content.ReadAsByteArrayAsync();
                clone.Content = new ByteArrayContent(contentBytes);
                foreach (var header in request.Content.Headers)
                {
                    clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }
            clone.Version = request.Version;
            return clone;
        }
    }
}
