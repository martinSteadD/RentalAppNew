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
            return await _api.GetAsync<List<Rental>>("rentals/my")
                    ?? new List<Rental>();
        }

        public async Task<List<Rental>> GetIncomingRentalsAsync()
        {
            var wrapper = await _api.GetAsync<IncomingRentalsResponse>("rentals/incoming");

            return wrapper?.Rentals ?? new List<Rental>();
        }


        public async Task<Rental?> RequestRentalAsync(int itemId)
        {
            var payload = new { itemId };
            return await _api.PostAsync<object, Rental>("rentals", payload);
        }

        public async Task<bool> ApproveRentalAsync(int rentalId)
        {
            var payload = new { status = "Approved" };
            var result = await _api.PatchAsync<object, object>($"rentals/{rentalId}/status", payload);
            return result != null;
        }

        public async Task<bool> CancelRentalAsync(int rentalId)
        {
            var payload = new { status = "Cancelled" };
            var result = await _api.PatchAsync<object, object>($"rentals/{rentalId}/status", payload);
            return result != null;
        }
    }
}
