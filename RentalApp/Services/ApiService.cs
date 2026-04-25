using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using RentalApp.Database.Models;


namespace RentalApp.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        private string? _token;

        public ApiService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://set09102-api.b-davison.workers.dev/")
            };

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public void SetToken(string? token)
        {
            _token = token;
        }

        private void ApplyAuthHeader(HttpRequestMessage request)
        {
            if (!string.IsNullOrWhiteSpace(_token))
            {
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", _token);
            }
        }

        public async Task<T?> GetAsync<T>(string endpoint)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
            ApplyAuthHeader(request);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json, _jsonOptions);
        }

        public async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = new StringContent(
                    JsonSerializer.Serialize(data),
                    Encoding.UTF8,
                    "application/json")
            };

            ApplyAuthHeader(request);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TResponse>(json, _jsonOptions);
        }

        public async Task<bool> DeleteAsync(string endpoint)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, endpoint);
            ApplyAuthHeader(request);

            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        public async Task<IEnumerable<Item>> GetItemsAsync()
        {
            var response = await _httpClient.GetAsync("items");

            if (!response.IsSuccessStatusCode)
                throw new Exception("Failed to load items");

            var json = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<PagedItemsResponse>(json, _jsonOptions);

            return result?.Items ?? Enumerable.Empty<Item>();
        }


        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            var response = await _httpClient.GetAsync("categories");

            if (!response.IsSuccessStatusCode)
                throw new Exception("Failed to load categories");

            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<IEnumerable<Category>>(json, _jsonOptions)
                ?? Enumerable.Empty<Category>();
        }

        public async Task<Item?> CreateItemAsync(Item item)
        {
            var json = JsonSerializer.Serialize(item, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("items", content);

            if (!response.IsSuccessStatusCode)
                return null;

            var responseJson = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<Item>(responseJson, _jsonOptions);
        }



    }
}
