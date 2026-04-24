using RentalApp.Database.Models;

namespace RentalApp.Services
{
    public class RentalService : IRentalService
    {
        private readonly IApiService _api;

        public RentalService(IApiService api)
        {
            _api = api;
        }

        public async Task<List<Rental>> GetMyRentalsAsync()
        {
            return await _api.GetAsync<List<Rental>>("rentals/my");
        }

        public async Task<List<Rental>> GetRentalsForItemAsync(int itemId)
        {
            return await _api.GetAsync<List<Rental>>($"rentals/item/{itemId}");
        }

        public async Task<Rental?> RequestRentalAsync(int itemId)
        {
            var payload = new { itemId };

            return await _api.PostAsync<object, Rental>("rentals/request", payload);
        }

        public async Task<bool> ApproveRentalAsync(int rentalId)
        {
            var result = await _api.PostAsync<object, object>($"rentals/{rentalId}/approve", new { });
            return result != null;
        }

        public async Task<bool> CancelRentalAsync(int rentalId)
        {
            return await _api.DeleteAsync($"rentals/{rentalId}");
        }
    }
}
