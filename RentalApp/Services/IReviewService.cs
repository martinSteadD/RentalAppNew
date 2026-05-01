using RentalApp.Database.Models;

namespace RentalApp.Services;

public interface IReviewService
{
    Task<ReviewsResponse?> GetReviewsForItemAsync(int itemId);
    Task<bool> SubmitReviewAsync(CreateReviewRequest request);
}


