using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Net.Http.Json;
using RentalApp.Models;
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
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public void SetToken(string? token)
        {
            _token = token;
        }
        public async Task EnsureTokenLoadedAsync()
        {
            if (!string.IsNullOrWhiteSpace(_token))
                return;

            var storedToken = await SecureStorage.GetAsync("auth_token");
            if (!string.IsNullOrWhiteSpace(storedToken))
                _token = storedToken;
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
            var json = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();

            return JsonSerializer.Deserialize<T>(json, _jsonOptions);
        }

        public async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = new StringContent(
                    JsonSerializer.Serialize(data, _jsonOptions),
                    Encoding.UTF8,
                    "application/json")
            };

            ApplyAuthHeader(request);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TResponse>(json, _jsonOptions);
        }

        public async Task<bool> PostAsync<TRequest>(string endpoint, TRequest data)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = new StringContent(
                    JsonSerializer.Serialize(data, _jsonOptions),
                    Encoding.UTF8,
                    "application/json")
            };

            ApplyAuthHeader(request);

            var response = await _httpClient.SendAsync(request);

            var body = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"[POST] URL: {endpoint}");
            Console.WriteLine($"[POST] Status: {response.StatusCode}");
            Console.WriteLine($"[POST] Body: {body}");

            return response.IsSuccessStatusCode;
        }



        public async Task<Item?> GetItemByIdAsync(int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"items/{id}");
            ApplyAuthHeader(request);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Item>(json, _jsonOptions);
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
            Console.WriteLine("🌐 [ApiService] GetItemsAsync CALLED");

            Console.WriteLine("🌐 [ApiService] Sending GET request to: items");
            var response = await _httpClient.GetAsync("items");

            Console.WriteLine($"🌐 [ApiService] Response status: {response.StatusCode}");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("❌ [ApiService] Response was NOT successful");
                throw new Exception("Failed to load items");
            }

            var json = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"📥 [ApiService] JSON RECEIVED: {json}");

            var result = JsonSerializer.Deserialize<PagedItemsResponse>(json, _jsonOptions);

            if (result == null)
            {
                Console.WriteLine("❌ [ApiService] Deserialization returned NULL");
            }
            else
            {
                Console.WriteLine($"📦 [ApiService] Deserialized {result.Items?.Count() ?? 0} items");
            }

            return result?.Items ?? Enumerable.Empty<Item>();
        }



        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            var response = await _httpClient.GetAsync("categories");

            if (!response.IsSuccessStatusCode)
                throw new Exception("Failed to load categories");

            var json = await response.Content.ReadAsStringAsync();

            var wrapper = JsonSerializer.Deserialize<CategoryResponse>(json, _jsonOptions);

            return wrapper?.Categories ?? Enumerable.Empty<Category>();
        }


        public async Task<Item?> CreateItemAsync(CreateItemRequest item)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "items")
            {
                Content = new StringContent(
                    JsonSerializer.Serialize(item, _jsonOptions),
                    Encoding.UTF8,
                    "application/json")
            };

            ApplyAuthHeader(request);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine("CREATE ITEM ERROR: " + error);
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Item>(json, _jsonOptions);
        }

        public async Task<TResponse?> PatchAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Patch, endpoint)
            {
                Content = content
            };

            ApplyAuthHeader(request);

            Console.WriteLine("DEBUG → PATCH URL: " + endpoint);
            Console.WriteLine("DEBUG → PATCH BODY: " + json);

            var response = await _httpClient.SendAsync(request);

            Console.WriteLine("DEBUG → RESPONSE STATUS: " + response.StatusCode);

            var responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine("DEBUG → RESPONSE BODY: " + responseBody);

            if (!response.IsSuccessStatusCode)
                return default;

            return JsonSerializer.Deserialize<TResponse>(responseBody, _jsonOptions);
        }

        public async Task<bool> UpdateItemAsync(int itemId, UpdateItemRequest request)
        {
            var response = await _httpClient.PatchAsJsonAsync($"items/{itemId}", request);
            return response.IsSuccessStatusCode;
        }


        public async Task<List<Item>> GetAllItemsAsync()
        {
            var allItems = new List<Item>();
            int page = 1;

            while (true)
            {
                var response = await _httpClient.GetAsync($"items?page={page}");

                if (!response.IsSuccessStatusCode)
                    break;

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<PagedItemsResponse>(json, _jsonOptions);

                if (result?.Items == null || result.Items.Count == 0)
                    break;

                allItems.AddRange(result.Items);
                page++;
            }

            return allItems;
        }

        public async Task<List<Item>> GetNearbyItemsAsync(double latitude, double longitude, int radius = 50)
        {
            var url = $"items/nearby?lat={latitude}&lon={longitude}&radius={radius}";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            ApplyAuthHeader(request);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                return new List<Item>();

            var json = await response.Content.ReadAsStringAsync();

            var responseObj = JsonSerializer.Deserialize<NearbyItemsResponse>(json, _jsonOptions);

            return responseObj?.Items ?? new List<Item>();
        }

    }
}
