using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using kafi.Models;

namespace kafi.Data;

public interface IInfoDao
{
    Task UpdateInfo(UserRequest request);
    Task UpdateProfileImage(MultipartFormDataContent image);
}

public class RestInfoDao(IHttpClientFactory httpClientFactory) : IInfoDao
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("Common");

    public async Task UpdateInfo(UserRequest request)
    {
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var response = await _httpClient.PatchAsync("users/user", content);
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateProfileImage(MultipartFormDataContent image)
    {
        var response = await _httpClient.PutAsync("users/user/image", image);
        response.EnsureSuccessStatusCode();
    }
}
