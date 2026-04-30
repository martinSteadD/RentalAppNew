using System.Text.Json;
using RentalApp.Database.Models;

namespace RentalApp.Services
{
    public class RentalService : IRentalService
    {
        private readonly IApiService _api;
        private readonly JsonSerializerOptions _options;

        public RentalService(IApiService api)
        {
            _api = api;

            _options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<List<Rental>> GetMyRentalsAsync()
        {
            var wrapper = await _api.GetAsync<OutgoingRentalsResponse>("rentals/outgoing");
            return wrapper?.Rentals ?? new List<Rental>();
        }


        public async Task<List<Rental>> GetIncomingRentalsAsync()
        {
            // ApiService already handles auth + base URL + HttpClient
            var wrapper = await _api.GetAsync<IncomingRentalsResponse>("rentals/incoming");

            return wrapper?.Rentals ?? new List<Rental>();
        }

        public async Task<Rental?> RequestRentalAsync(int itemId)
        {
            // Ensure token exists
            await _api.EnsureTokenLoadedAsync();


            //  Build payload
            var payload = new 
            { 
                itemId = itemId,
                startDate = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                endDate = DateTime.UtcNow.AddDays(3).ToString("yyyy-MM-dd")
            };

            // Send request
            return await _api.PostAsync<object, Rental>("rentals", payload);
        }


        public async Task<bool> UpdateRentalStatusAsync(int rentalId, string status)
        {
            var payload = new { status };
            var result = await _api.PatchAsync<object, object>($"rentals/{rentalId}/status", payload);
            return result != null;
        }

        public Task<bool> ApproveRentalAsync(int rentalId)
            => UpdateRentalStatusAsync(rentalId, "Approved");

        public Task<bool> CancelRentalAsync(int rentalId)
            => UpdateRentalStatusAsync(rentalId, "Cancelled");

        public Task<bool> RejectRentalAsync(int rentalId)
            => UpdateRentalStatusAsync(rentalId, "Rejected");
        
        public Task<bool> ReturnRentalAsync(int rentalId)
        => UpdateRentalStatusAsync(rentalId, "Returned");

    }
}
