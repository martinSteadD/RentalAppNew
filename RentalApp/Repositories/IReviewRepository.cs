using RentalApp.Database.Models;

namespace RentalApp.Repositories
{
    public interface IReviewRepository
    {
        Task<IEnumerable<Review>> GetReviewsForItemAsync(int itemId);
        Task<bool> AddReviewAsync(int itemId, CreateReviewRequest request);
    }
}
