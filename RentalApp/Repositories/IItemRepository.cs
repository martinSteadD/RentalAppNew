using RentalApp.Database.Models;

namespace RentalApp.Repositories
{
    public interface IItemRepository : IRepository<Item>
    {
        Task<IEnumerable<Category>> GetCategoriesAsync();
        // No extra methods needed yet — inherits:
        // GetAllAsync()
        // GetByIdAsync()
        // AddAsync()
        // UpdateAsync()
        // DeleteAsync()
    }
}
