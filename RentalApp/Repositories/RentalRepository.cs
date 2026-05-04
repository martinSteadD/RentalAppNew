using RentalApp.Database.Models;
using RentalApp.Services;

namespace RentalApp.Repositories
{
    public class RentalRepository : IRentalRepository
    {
        private readonly IApiService _apiService;

        public RentalRepository(IApiService apiService)
        {
            _apiService = apiService;
        }

        // Borrower: rentals I have requested
        public async Task<List<Rental>> GetOutgoingRentalsAsync()
        {
            var response = await _apiService.GetAsync<OutgoingRentalsResponse>("rentals/outgoing");
            return response?.Rentals ?? new List<Rental>();
        }

        // Owner: rentals requested for items I own
        public async Task<List<Rental>> GetIncomingRentalsAsync()
        {
            var response = await _apiService.GetAsync<IncomingRentalsResponse>("rentals/incoming");

            if (response == null || response.Rentals == null)
                return new List<Rental>();

            return response.Rentals;
        }

        // Borrower: request a rental
        public async Task<Rental?> RequestRentalAsync(RentalRequest request)
        {
            var success = await _apiService.PostAsync("rentals", request);

            if (!success)
                return null;

            return new Rental
            {
                ItemId = request.itemId,
                StartDate = DateTime.Parse(request.startDate),
                EndDate = DateTime.Parse(request.endDate),
                Status = "pending"
            };
        }

        // Owner or borrower: update rental status
        public async Task<bool> UpdateRentalStatusAsync(int rentalId, string status)
        {
            var body = new { status };
            return await _apiService.PatchAsync<object, bool>($"rentals/{rentalId}/status", body);
        }

        public Task<bool> ApproveRentalAsync(int rentalId)
            => UpdateRentalStatusAsync(rentalId, "approved");

        public Task<bool> RejectRentalAsync(int rentalId)
            => UpdateRentalStatusAsync(rentalId, "rejected");

        public Task<bool> CancelRentalAsync(int rentalId)
            => UpdateRentalStatusAsync(rentalId, "cancelled");

        // ⭐ NEW: Rentals for the logged‑in user (borrower)
        // We use this instead of the broken /incoming endpoint
        public async Task<List<Rental>> GetMyRentalsAsync()
        {
            var response = await _apiService.GetAsync<OutgoingRentalsResponse>("rentals/outgoing");

            if (response == null)
            {
                Console.WriteLine("📥 RAW RENTALS RESPONSE: null");
                return new List<Rental>();
            }

            Console.WriteLine("📥 RAW RENTALS RESPONSE: " + (response.Rentals?.Count ?? 0));

            return response.Rentals ?? new List<Rental>();
        }

        public async Task<List<Rental>> GetOwnerIncomingAsync(int ownerId)
        {
            // Load from the ONLY working endpoint
            var response = await _apiService.GetAsync<OutgoingRentalsResponse>("rentals/outgoing");

            if (response?.Rentals == null)
                return new List<Rental>();

            // Filter for owner-side requests
            return response.Rentals
                .Where(r =>
                    r.OwnerId == ownerId &&
                    (r.Status?.ToLower() == "requested" ||
                    r.Status?.ToLower() == "pending"))
                .ToList();
        }

    }
}
