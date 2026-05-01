using RentalApp.Database.Models;

namespace RentalApp.Services;

public class ReviewService : IReviewService
{
    private readonly IApiService _api;

    public ReviewService(IApiService api)
    {
        _api = api;
    }

    // ⭐ FIXED: Return ReviewsResponse, not List<Review>
    public async Task<ReviewsResponse?> GetReviewsForItemAsync(int itemId)
    {
        return await _api.GetAsync<ReviewsResponse>($"items/{itemId}/reviews");
    }

    public async Task<bool> SubmitReviewAsync(CreateReviewRequest request)
    {
        return await _api.PostAsync("reviews", request);
    }
}
