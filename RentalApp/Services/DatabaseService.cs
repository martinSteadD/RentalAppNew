using SQLite;
using RentalApp.Models;

namespace RentalApp.Services
{
    public class DatabaseService
    {
        private readonly SQLiteAsyncConnection _db;

        public DatabaseService()
        {
            var dbPath = Path.Combine(
                FileSystem.AppDataDirectory,
                "rentalapp.db3"
            );

            _db = new SQLiteAsyncConnection(dbPath);
        }

        // ---------------------------------------------------------
        // INITIALIZATION
        // ---------------------------------------------------------
        public async Task InitializeAsync()
        {
            await _db.CreateTableAsync<LocalItem>();
            await _db.CreateTableAsync<LocalRental>();
        }

        // ---------------------------------------------------------
        // ITEM OPERATIONS (UPSERT)
        // ---------------------------------------------------------
        public async Task<int> SaveItemAsync(LocalItem item)
        {
            var existing = await _db.Table<LocalItem>()
                .Where(i => i.ApiItemId == item.ApiItemId)
                .FirstOrDefaultAsync();

            if (existing == null)
            {
                return await _db.InsertAsync(item);
            }
            else
            {
                existing.Title = item.Title;
                existing.Description = item.Description;
                existing.ImageUrl = item.ImageUrl;

                existing.CategoryId = item.CategoryId;
                existing.Category = item.Category;

                existing.DailyRate = item.DailyRate;
                existing.IsAvailable = item.IsAvailable;

                existing.OwnerName = item.OwnerName;
                existing.OwnerRating = item.OwnerRating;

                existing.AverageRating = item.AverageRating;

                existing.Latitude = item.Latitude;
                existing.Longitude = item.Longitude;

                existing.LastSynced = DateTime.UtcNow;

                return await _db.UpdateAsync(existing);
            }
        }

        public Task<List<LocalItem>> GetAllItemsAsync()
        {
            return _db.Table<LocalItem>().ToListAsync();
        }

        public Task<LocalItem> GetItemByApiIdAsync(int apiItemId)
        {
            return _db.Table<LocalItem>()
                .Where(i => i.ApiItemId == apiItemId)
                .FirstOrDefaultAsync();
        }

        // ---------------------------------------------------------
        // RENTAL OPERATIONS (UPSERT)
        // ---------------------------------------------------------
        public async Task<int> SaveRentalAsync(LocalRental rental)
        {
            var existing = await _db.Table<LocalRental>()
                .Where(r => r.ApiRentalId == rental.ApiRentalId)
                .FirstOrDefaultAsync();

            if (existing == null)
            {
                return await _db.InsertAsync(rental);
            }
            else
            {
                existing.Status = rental.Status;
                existing.BorrowerId = rental.BorrowerId;
                existing.RequestedBy = rental.RequestedBy;

                existing.StartDate = rental.StartDate;
                existing.EndDate = rental.EndDate;

                existing.ApiItemId = rental.ApiItemId;

                existing.LastSynced = DateTime.UtcNow;

                return await _db.UpdateAsync(existing);
            }
        }

        public Task<List<LocalRental>> GetAllRentalsAsync()
        {
            return _db.Table<LocalRental>().ToListAsync();
        }

        public Task<LocalRental> GetRentalByApiIdAsync(int apiRentalId)
        {
            return _db.Table<LocalRental>()
                .Where(r => r.ApiRentalId == apiRentalId)
                .FirstOrDefaultAsync();
        }

        public Task<List<LocalRental>> GetRentalsForItemAsync(int apiItemId)
        {
            return _db.Table<LocalRental>()
                .Where(r => r.ApiItemId == apiItemId)
                .ToListAsync();
        }

        // ---------------------------------------------------------
        // CLEANUP HELPERS (FIXED)
        // ---------------------------------------------------------
        public Task DeleteItemsNotOwnedByAsync(int userId)
        {
            return _db.Table<LocalItem>()
                    .Where(i => i.CreatedBy != userId)
                    .DeleteAsync();
        }

        // Do NOT delete rentals where BorrowerId = 0
        // because the API loses borrowerId on older rentals.
        public Task DeleteRentalsNotForBorrowerAsync(int borrowerId)
        {
            return _db.Table<LocalRental>()
                .Where(r => r.BorrowerId != borrowerId && r.BorrowerId != 0)
                .DeleteAsync();
        }

        public async Task ClearAllAsync()
        {
            await _db.DeleteAllAsync<LocalItem>();
            await _db.DeleteAllAsync<LocalRental>();
        }
    }
}
