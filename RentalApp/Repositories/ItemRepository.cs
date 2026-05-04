using RentalApp.Database.Models;
using RentalApp.Services;

namespace RentalApp.Repositories
{
    public class ItemRepository : IItemRepository
    {
        private readonly IApiService _api;

        public ItemRepository(IApiService api)
        {
            _api = api;
        }

        public async Task<List<Item>> GetAllAsync()
        {
            var apiItems = await _api.GetItemsAsync();

            return apiItems.Select(i => new Item
            {
                Id = i.Id,
                Title = i.Title,
                Description = i.Description,
                DailyRate = i.DailyRate,
                CategoryId = i.CategoryId,
                Category = i.Category,
                OwnerId = i.OwnerId,
                OwnerName = i.OwnerName,
                OwnerRating = i.OwnerRating,
                IsAvailable = i.IsAvailable,
                AverageRating = i.AverageRating,
                ImageUrl = i.ImageUrl,
                CreatedAt = i.CreatedAt,
                Latitude = i.Latitude,
                Longitude = i.Longitude
            }).ToList();
        }

        public Task<Item?> GetByIdAsync(int id) =>
            _api.GetItemByIdAsync(id);

        public Task<Item?> CreateAsync(Item entity)
        {
            var request = new CreateItemRequest
            {
                Title = entity.Title,
                Description = entity.Description,
                DailyRate = entity.DailyRate,
                CategoryId = entity.CategoryId,   // <-- now correct
                Latitude = entity.Latitude ?? 0,
                Longitude = entity.Longitude ?? 0
            };

            return _api.CreateItemAsync(request);
        }

        public async Task<Item?> UpdateAsync(Item entity)
        {
            var updated = await _api.UpdateAsync(entity);
            return updated;
        }

        public Task<bool> DeleteAsync(int id) =>
            _api.DeleteAsync($"items/{id}");

        // ⭐ NEW: Load categories from the API properly
        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            return await _api.GetCategoriesAsync();
        }
    }
}
