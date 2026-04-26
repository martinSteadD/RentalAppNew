using RentalApp.Database.Models;

namespace RentalApp.Services
{
    public interface IRentalService
    {
        Task<List<Rental>> GetMyRentalsAsync();
        Task<List<Rental>> GetIncomingRentalsAsync();
        Task<Rental?> RequestRentalAsync(int itemId);
        Task<bool> ApproveRentalAsync(int rentalId);
        Task<bool> CancelRentalAsync(int rentalId);
    }
}
