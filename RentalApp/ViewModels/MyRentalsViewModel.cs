using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RentalApp.Models;
using RentalApp.Services;
using System.Collections.ObjectModel;

namespace RentalApp.ViewModels;

public partial class MyRentalsViewModel : BaseViewModel
{
    private readonly IRentalService _rentalService;
    private readonly INavigationService _navigationService;
    private readonly DatabaseService _databaseService;
    private readonly IAuthenticationService _authService;
    private readonly IApiService _api;

    [ObservableProperty]
    private ObservableCollection<RentalDisplayModel> rentals = new();

    public MyRentalsViewModel(
        IRentalService rentalService,
        INavigationService navigationService,
        DatabaseService databaseService,
        IAuthenticationService authService,
        IApiService apiService)
    {
        _rentalService = rentalService;
        _navigationService = navigationService;
        _databaseService = databaseService;
        _authService = authService;
        _api = apiService;

        Title = "My Rentals";
    }

    [RelayCommand]
    private async Task LoadRentalsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            ClearError();

            int userId = _authService.CurrentUser!.Id;

            Console.WriteLine($"[MyRentals] Loading rentals for borrowerId={userId}");

            // 1️⃣ Pull rentals from API
            var apiRentals = await _rentalService.GetMyRentalsAsync();
            Console.WriteLine($"[MyRentals] API returned {apiRentals.Count} rentals");

            // 2️⃣ FIX: Keep rentals where BorrowerId is missing (0) or matches user
            apiRentals = apiRentals
                .Where(r => r.BorrowerId == userId || r.BorrowerId == 0)
                .ToList();

            Console.WriteLine($"[MyRentals] After filtering, {apiRentals.Count} rentals belong to this borrower");

            // 3️⃣ Only delete local rentals if API returned some
            if (apiRentals.Any())
            {
                Console.WriteLine("[MyRentals] API returned rentals → syncing + deleting stale ones");
                await _databaseService.DeleteRentalsNotForBorrowerAsync(userId);
            }
            else
            {
                Console.WriteLine("[MyRentals] API returned ZERO rentals → keeping local rentals");
            }

            // 4️⃣ Sync rentals from API into SQLite
            foreach (var rental in apiRentals)
            {
                Console.WriteLine($"[MyRentals] Syncing rental {rental.Id} (Item {rental.ItemId})");

                var localRental = new LocalRental
                {
                    ApiRentalId = rental.Id,
                    ApiItemId = rental.ItemId,
                    BorrowerId = rental.BorrowerId == 0 ? userId : rental.BorrowerId, // FIX
                    RequestedBy = rental.BorrowerId == 0 ? userId : rental.BorrowerId,
                    Status = rental.Status,
                    StartDate = rental.StartDate,
                    EndDate = rental.EndDate,
                    LastSynced = DateTime.UtcNow
                };

                await _databaseService.SaveRentalAsync(localRental);

                // Sync item
                var apiItem = await _api.GetItemByIdAsync(rental.ItemId);
                if (apiItem != null)
                {
                    var localItem = new LocalItem
                    {
                        ApiItemId = apiItem.Id,
                        CreatedBy = apiItem.CreatedBy,
                        Title = apiItem.Title,
                        Description = apiItem.Description,
                        ImageUrl = apiItem.ImageUrl,
                        CategoryId = apiItem.CategoryId,
                        Category = apiItem.Category,
                        DailyRate = apiItem.DailyRate,
                        IsAvailable = apiItem.IsAvailable,
                        OwnerName = apiItem.OwnerName,
                        OwnerRating = apiItem.OwnerRating ?? 0,
                        AverageRating = apiItem.AverageRating ?? 0,
                        CreatedAt = apiItem.CreatedAt,
                        LastSynced = DateTime.UtcNow
                    };

                    await _databaseService.SaveItemAsync(localItem);
                }
                else
                {
                    Console.WriteLine($"[MyRentals] WARNING: Item {rental.ItemId} not found in API");
                }
            }

            // 5️⃣ Load rentals from SQLite
            var myRentals = (await _databaseService.GetAllRentalsAsync())
                .Where(r => r.BorrowerId == userId)
                .OrderByDescending(r => r.ApiRentalId)
                .ToList();

            Console.WriteLine($"[MyRentals] SQLite returned {myRentals.Count} rentals for borrower");

            // 6️⃣ Load items for join
            var allItems = await _databaseService.GetAllItemsAsync();

            Rentals.Clear();

            foreach (var rental in myRentals)
            {
                var item = allItems.FirstOrDefault(i => i.ApiItemId == rental.ApiItemId);

                if (item == null)
                {
                    Console.WriteLine($"[MyRentals] WARNING: Missing item for rental {rental.ApiRentalId}");

                    Rentals.Add(new RentalDisplayModel
                    {
                        Rental = rental,
                        ItemTitle = "Unknown Item",
                        ItemImage = null,
                        Category = "Unknown",
                        DailyRate = 0
                    });

                    continue;
                }

                Rentals.Add(new RentalDisplayModel
                {
                    Rental = rental,
                    ItemTitle = item.Title,
                    ItemImage = item.ImageUrl,
                    Category = item.Category,
                    DailyRate = item.DailyRate
                });
            }
        }
        catch (Exception ex)
        {
            SetError($"Failed to load rentals: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task RefreshAsync() => await LoadRentalsAsync();

    [RelayCommand]
    private async Task NavigateBackAsync() => await _navigationService.NavigateBackAsync();

    [RelayCommand]
    private async Task MarkReturnedAsync(int rentalId)
    {
        try
        {
            IsBusy = true;
            ClearError();

            Console.WriteLine($"[MyRentals] MarkReturned called for rental {rentalId}");

            await _rentalService.ReturnRentalAsync(rentalId);

            var rental = await _databaseService.GetRentalByApiIdAsync(rentalId);
            if (rental != null)
            {
                rental.Status = "Returned";
                rental.LastSynced = DateTime.UtcNow;
                await _databaseService.SaveRentalAsync(rental);
            }

            await LoadRentalsAsync();
        }
        catch (Exception ex)
        {
            SetError($"Failed to mark returned: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }

    }

    [RelayCommand]
    private async Task AddReviewAsync(int rentalId)
    {
        var rental = Rentals.FirstOrDefault(r => r.Rental.ApiRentalId == rentalId);

        if (rental == null)
        {
            Console.WriteLine("[DEBUG] Rental not found in Rentals list.");
            return;
        }

        // ⭐ THIS IS THE IMPORTANT LOG ⭐
        Console.WriteLine($"[DEBUG] RentalId={rental.Rental.ApiRentalId}, Status={rental.Rental.Status}");

        await _navigationService.NavigateToAsync("addreview", new Dictionary<string, object>
        {
            { "RentalId", rental.Rental.ApiRentalId }
        });
    }


}
