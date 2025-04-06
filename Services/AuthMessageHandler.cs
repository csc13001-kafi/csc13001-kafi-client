using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using kafi.Contracts.Services;
using kafi.Models;

namespace kafi.Services;

public partial class AuthMessageHandler(ISecureTokenStorage secureTokenStorage) : DelegatingHandler
{
    private readonly ISecureTokenStorage _tokenStorage = secureTokenStorage;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var (accessToken, refreshToken) = _tokenStorage.GetTokens();

        if (!string.IsNullOrEmpty(accessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }
        var response = await base.SendAsync(request, cancellationToken);

        Debug.WriteLine($"Initial response: {response.StatusCode}");
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            if (!string.IsNullOrEmpty(refreshToken))
            {
                bool refreshed = await TryRefreshTokenAsync(refreshToken);
                Debug.WriteLine($"Refresh succeeded: {refreshed}");
                if (refreshed)
                {
                    var clonedRequest = await CloneHttpRequestMessageAsync(request);
                    (accessToken, _) = _tokenStorage.GetTokens();
                    Debug.WriteLine($"Retrying with accessToken: {accessToken}");
                    clonedRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    response = await base.SendAsync(clonedRequest, cancellationToken);
                    Debug.WriteLine($"Retry response: {response.StatusCode}");
                }
            }
        }
        return response;
    }

    private async Task<bool> TryRefreshTokenAsync(string refreshToken)
    {
        using var client = new HttpClient();
        client.BaseAddress = new Uri("http://localhost:8080");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", refreshToken);
        try
        {
            var response = await client.GetAsync("auth/refresh-token");
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var refreshResponse = JsonSerializer.Deserialize<RefreshTokenResponse>(responseContent)!;
                _tokenStorage.SaveTokens(refreshResponse.AccessToken, refreshToken);
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to refresh token, error {ex}");
            return false;
        }
    }

    private static async Task<HttpRequestMessage> CloneHttpRequestMessageAsync(HttpRequestMessage request)
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
