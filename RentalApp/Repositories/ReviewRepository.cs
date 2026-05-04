using RentalApp.Database.Models;
using RentalApp.Services;

namespace RentalApp.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly IApiService _apiService;

        public ReviewRepository(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IEnumerable<Review>> GetReviewsForItemAsync(int itemId)
        {
            return await _apiService.GetAsync<IEnumerable<Review>>($"items/{itemId}/reviews")
                   ?? Enumerable.Empty<Review>();
        }

        public async Task<bool> AddReviewAsync(int itemId, CreateReviewRequest request)
        {
            return await _apiService.PostAsync($"items/{itemId}/reviews", request);
        }
    }
}
