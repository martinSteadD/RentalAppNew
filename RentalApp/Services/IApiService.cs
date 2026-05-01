using RentalApp.Database.Models;
using RentalApp.Models;


namespace RentalApp.Services

{
    public interface IApiService
    {
        Task<IEnumerable<Item>> GetItemsAsync();
        Task<List<Item>> GetAllItemsAsync();
        Task<Item?> GetItemByIdAsync(int id);
        Task<IEnumerable<Category>> GetCategoriesAsync();
        Task<Item?> CreateItemAsync(CreateItemRequest item);
        Task<T?> GetAsync<T>(string endpoint);
        Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data);
        Task<bool> PostAsync<TRequest>(string endpoint, TRequest data);
        Task<TResponse?> PatchAsync<TRequest, TResponse>(string url, TRequest body);
        Task<bool> DeleteAsync(string endpoint);
        Task<bool> UpdateItemAsync(int itemId, UpdateItemRequest request);
        Task EnsureTokenLoadedAsync();
        void SetToken(string? token);
        Task<List<Item>> GetNearbyItemsAsync(double latitude, double longitude, int radius);

    }
}
