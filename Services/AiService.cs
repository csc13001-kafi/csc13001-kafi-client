using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using kafi.Contracts.Services;
using kafi.Models;

namespace kafi.Services;

public class AiService(IHttpClientFactory httpClientFactory) : IAiService
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("Common");

    public async Task<Message> SendMessageAsync(string sessionId, string message)
    {
        var json = JsonSerializer.Serialize(new Message { Text = message });
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        content.Headers.Add("sessionId", sessionId);
        var response = await _httpClient.PostAsync($"/ai/chat", content);
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<Message>(responseContent) ?? throw new Exception("Failed to deserialize response");
    }

    public async Task<string> StartNewChatAsync()
    {
        var response = await _httpClient.PostAsync("/ai/session", null);
        response.EnsureSuccessStatusCode();
        var sessionId = await response.Content.ReadAsStringAsync();
        return sessionId;
    }
}
