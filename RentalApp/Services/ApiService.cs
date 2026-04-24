using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace RentalApp.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public string? AuthToken { get; private set; }

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

        private void ApplyAuthHeader()
        {
            if (!string.IsNullOrEmpty(AuthToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", AuthToken);
            }
        }

        public async Task<T> GetAsync<T>(string endpoint)
        {
            ApplyAuthHeader();

            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json, _jsonOptions)!;
        }

        public async Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            ApplyAuthHeader();

            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(endpoint, content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TResponse>(responseJson, _jsonOptions)!;
        }

        public async Task<TResponse> PutAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            ApplyAuthHeader();

            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(endpoint, content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TResponse>(responseJson, _jsonOptions)!;
        }

        public async Task<bool> DeleteAsync(string endpoint)
        {
            ApplyAuthHeader();

            var response = await _httpClient.DeleteAsync(endpoint);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            var payload = new { email, password };

            var result = await PostAsync<object, AuthResult>("auth/login", payload);

            if (result.Success)
            {
                AuthToken = result.Token;
                return true;
            }

            return false;
        }

        public async Task<bool> RegisterAsync(string email, string password, string firstName, string lastName)
        {
            var payload = new { email, password, firstName, lastName };

            var result = await PostAsync<object, AuthResult>("auth/register", payload);

            if (result.Success)
            {
                AuthToken = result.Token;
                return true;
            }

            return false;
        }
    }
}
