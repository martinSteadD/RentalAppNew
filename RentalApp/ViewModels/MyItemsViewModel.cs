using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RentalApp.Models;
using RentalApp.Services;
using System.Collections.ObjectModel;

namespace RentalApp.ViewModels;

public partial class MyItemsViewModel : ObservableObject
{
    private readonly IApiService _api;
    private readonly IRentalService _rentalService;
    private readonly IAuthenticationService _auth;
    private readonly DatabaseService _database;

    [ObservableProperty]
    private ObservableCollection<LocalItem> availableItems = new();

    [ObservableProperty]
    private ObservableCollection<MyItemRentalDisplay> rentedOutItems = new();

    [ObservableProperty]
    private bool isBusy;

    public MyItemsViewModel(
        IApiService api,
        IRentalService rentalService,
        IAuthenticationService auth,
        DatabaseService database)
    {
        _api = api;
        _rentalService = rentalService;
        _auth = auth;
        _database = database;
    }

    // ---------------------------------------------------------
    // LOAD ITEMS + RENTALS
    // ---------------------------------------------------------
    [RelayCommand]
    public async Task LoadItemsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            int userId = _auth.CurrentUser!.Id;

            // 1) SYNC ITEMS FROM API → SQLITE
            var apiItems = await _api.GetItemsAsync();
            var myApiItems = apiItems.Where(i => i.CreatedBy == userId).ToList();

            foreach (var item in myApiItems)
            {
                var local = new LocalItem
                {
                    ApiItemId = item.Id,
                    CreatedBy = item.CreatedBy,
                    Title = item.Title,
                    Description = item.Description,
                    ImageUrl = item.ImageUrl,
                    CategoryId = item.CategoryId,
                    Category = item.Category,
                    DailyRate = item.DailyRate,
                    IsAvailable = item.IsAvailable,
                    OwnerName = item.OwnerName,
                    OwnerRating = item.OwnerRating ?? 0,
                    AverageRating = item.AverageRating ?? 0,
                    CreatedAt = item.CreatedAt,
                    LastSynced = DateTime.UtcNow
                };

                await _database.SaveItemAsync(local);
            }

            var localItems = await _database.GetAllItemsAsync();
            var myItems = localItems.Where(i => i.CreatedBy == userId).ToList();

            // 2) SYNC RENTALS FOR YOUR ITEMS (API → LocalRental)
            var apiIncoming = await _rentalService.GetIncomingRentalsAsync();

            var myApiRentals = apiIncoming
                .Where(r => myItems.Any(i => i.ApiItemId == r.ItemId))
                .ToList();

            foreach (var rental in myApiRentals)
            {
                var localRental = new LocalRental
                {
                    ApiRentalId = rental.Id,
                    ApiItemId = rental.ItemId,
                    BorrowerId = rental.BorrowerId,
                    RequestedBy = rental.BorrowerId,
                    Status = rental.Status,
                    StartDate = rental.StartDate,
                    EndDate = rental.EndDate,
                    LastSynced = DateTime.UtcNow
                };

                await _database.SaveRentalAsync(localRental);
            }

            var localRentals = await _database.GetAllRentalsAsync();

            // 3) BUILD UI LISTS
            AvailableItems.Clear();
            RentedOutItems.Clear();

           foreach (var item in myItems)
{
            var rental = myApiRentals
                .Where(r => r.ItemId == item.ApiItemId)
                .OrderByDescending(r => r.StartDate)
                .FirstOrDefault();

            // If no rental OR rental is completed → item is available
            if (rental == null || rental.Status == "Completed")
            {
                AvailableItems.Add(item);
                continue;
            }

            // If rental is requested → still available
            if (rental.Status == "Requested")
            {
                AvailableItems.Add(item);
                continue;
            }

            // Otherwise → rented out
            var display = new MyItemRentalDisplay
            {
                Item = item,
                Rental = rental
            };

            RentedOutItems.Add(display);
        }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] LoadItemsAsync failed: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    // ---------------------------------------------------------
    // NAVIGATION
    // ---------------------------------------------------------
    [RelayCommand]
    public async Task OpenRentalDetailsAsync(MyItemRentalDisplay display)
    {
        // Convert API Rental → LocalRental before navigation
        var localRental = await _database.GetRentalByApiIdAsync(display.Rental.Id);

        await Shell.Current.GoToAsync("rentaldetails", new Dictionary<string, object>
        {
            { "Rental", localRental },
            { "Item", display.Item }
        });
    }

    [RelayCommand]
    public async Task OpenRentalRequestsAsync()
    {
        await Shell.Current.GoToAsync("rentalrequests");
    }
}
