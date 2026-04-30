using RentalApp.Database.Models;

namespace RentalApp.Services
{
    public interface IAuthenticationService
    {
        string? Token { get; }

        Task<string?> LoginAsync(string email, string password);
        Task<bool> RegisterAsync(string firstName, string lastName, string email, string password);

        // MUST return ApiUser, not User
        Task<ApiUser?> GetProfileAsync();

        Task LogoutAsync();

        ApiUser? CurrentUser { get; }
        int CurrentUserId { get; }
    }
}
