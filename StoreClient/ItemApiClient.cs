using System.Net;
using System.Net.Http.Json;
namespace StoreClient;

public class ItemApiClient
{
    // Must match the URL the API is running on
    private readonly HttpClient _http = new() { BaseAddress = new Uri("http://localhost:5000/") };
    public async Task<List<Item>> GetAllAsync(string? search = null)
    {
        var url = string.IsNullOrWhiteSpace(search) ? "api/items" :
            $"api/items?search={Uri.EscapeDataString(search)}";
        return await _http.GetFromJsonAsync<List<Item>>(url) ?? new();
    }
    public async Task<string?> AddAsync(Item item)
    {
        var res = await _http.PostAsJsonAsync("api/items", item);
        return res.IsSuccessStatusCode ? null : await res.Content.ReadAsStringAsync();
    }
    public async Task<string?> UpdateAsync(Item item)
    {
        var res = await _http.PutAsJsonAsync($"api/items/{Uri.EscapeDataString(item.Code)}", item);
        return res.IsSuccessStatusCode ? null : await res.Content.ReadAsStringAsync();
    }
    public async Task<bool> DeleteAsync(string code)
    {
        var res = await _http.DeleteAsync($"api/items/{Uri.EscapeDataString(code)}");
        return res.StatusCode == HttpStatusCode.NoContent;
    }
}
