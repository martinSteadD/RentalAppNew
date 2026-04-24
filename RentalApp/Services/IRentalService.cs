using RentalApp.Database.Models;

namespace RentalApp.Services
{
    public interface IRentalService
    {
        Task<List<Rental>> GetMyRentalsAsync();
        Task<List<Rental>> GetRentalsForItemAsync(int itemId);
        Task<Rental?> RequestRentalAsync(int itemId, DateOnly startDate, DateOnly endDate);
        Task<bool> ApproveRentalAsync(int rentalId);
        Task<bool> CancelRentalAsync(int rentalId);
    }
}
