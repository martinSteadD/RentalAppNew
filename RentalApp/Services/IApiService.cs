using RentalApp.Database.Models;


namespace RentalApp.Services

{
    public interface IApiService
    {
        Task<IEnumerable<Item>> GetItemsAsync();
        Task<IEnumerable<Category>> GetCategoriesAsync();
        Task<Item?> CreateItemAsync(CreateItemRequest item);
        Task<T?> GetAsync<T>(string endpoint);
        Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data);
        Task<TResponse?> PatchAsync<TRequest, TResponse>(string url, TRequest body);
        Task<bool> DeleteAsync(string endpoint);

        void SetToken(string? token);
    }
}
