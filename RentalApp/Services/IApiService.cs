namespace RentalApp.Services
{
    public interface IApiService
    {
        Task<T> GetAsync<T>(string endpoint);
        Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest data);
        Task<TResponse> PutAsync<TRequest, TResponse>(string endpoint, TRequest data);
        Task<bool> DeleteAsync(string endpoint);

        Task<bool> LoginAsync(string email, string password);
        Task<bool> RegisterAsync(string email, string password, string firstName, string lastName);

        string? AuthToken { get; }
    }
}
