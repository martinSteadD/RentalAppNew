namespace RentalApp.Repositories
{
    public interface IRepository<T>
    {
        Task<List<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task<T?> CreateAsync(T entity);
        Task<T?> UpdateAsync(T entity);
        Task<bool> DeleteAsync(int id);
    }
}
