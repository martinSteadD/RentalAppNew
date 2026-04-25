using RentalApp.Database.Models;

namespace RentalApp.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IApiService _api;

        public string? Token { get; private set; }

        public User? CurrentUser { get; private set; }

        public AuthenticationService(IApiService api)
        {
            _api = api;
        }

        public async Task<string?> LoginAsync(string email, string password)
        {
            var payload = new { email, password };

            var response = await _api.PostAsync<object, LoginResponse>("auth/token", payload);

            if (response == null || string.IsNullOrWhiteSpace(response.Token))
                return null;

            Token = response.Token;
            _api.SetToken(Token);

            // Optionally load user profile
            CurrentUser = await GetProfileAsync();

            return Token;
        }


        public async Task<bool> RegisterAsync(string firstName, string lastName, string email, string password)
        {
            var payload = new
            {
                firstName,
                lastName,
                email,
                password
            };

            var result = await _api.PostAsync<object, object>("auth/register", payload);

            return result != null;
        }

        public async Task<User?> GetProfileAsync()
        {
            return await _api.GetAsync<User>("users/me");
        }

        public Task LogoutAsync()
        {
            Token = null;
            CurrentUser = null;
            _api.SetToken(null);
            return Task.CompletedTask;
        }
    }

    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
    }
}
