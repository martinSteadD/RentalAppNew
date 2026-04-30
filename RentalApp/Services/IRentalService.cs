using RentalApp.Database.Models;

namespace RentalApp.Services
{
    public interface IRentalService
    {
        Task<List<Rental>> GetMyRentalsAsync();
        Task<List<Rental>> GetIncomingRentalsAsync();
        Task<bool> UpdateRentalStatusAsync(int rentalId, string status);
        Task<Rental?> RequestRentalAsync(int itemId);
        Task<bool> ApproveRentalAsync(int rentalId);
        Task<bool> CancelRentalAsync(int rentalId);
        Task<bool> ReturnRentalAsync(int rentalId);
    }
}
