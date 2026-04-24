using RentalApp.Database.Models;

namespace RentalApp.Services
{
    public interface IAuthenticationService
    {
        string? Token { get; }

        Task<bool> LoginAsync(string email, string password);
        Task<bool> RegisterAsync(string firstName, string lastName, string email, string password);
        Task<User?> GetProfileAsync();
        Task LogoutAsync();

        User? CurrentUser { get; }

    }
}
