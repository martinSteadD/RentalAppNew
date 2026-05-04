using RentalApp.Database.Models;

namespace RentalApp.Repositories
{
    public interface IRentalRepository
    {
        // Borrower: rentals I have requested
        Task<List<Rental>> GetOutgoingRentalsAsync();

        // Owner: rentals requested for items I own
        Task<List<Rental>> GetIncomingRentalsAsync();
        Task<List<Rental>> GetMyRentalsAsync();

        Task<List<Rental>> GetOwnerIncomingAsync(int ownerId);

        // Borrower: request a rental
        Task<Rental?> RequestRentalAsync(RentalRequest request);

        // Owner or borrower: update rental status
        Task<bool> UpdateRentalStatusAsync(int rentalId, string status);

        Task<bool> ApproveRentalAsync(int rentalId);
        Task<bool> RejectRentalAsync(int rentalId);
        Task<bool> CancelRentalAsync(int rentalId);
    }
}
