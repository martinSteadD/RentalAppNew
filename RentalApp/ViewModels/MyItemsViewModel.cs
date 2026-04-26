using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RentalApp.Services;
using RentalApp.Database.Models;
using System.Collections.ObjectModel;

namespace RentalApp.ViewModels
{
    public partial class MyItemsViewModel : ObservableObject
    {
        private readonly IApiService _apiService;
        private readonly IRentalService _rentalService;
        private readonly IAuthenticationService _authService;
        private readonly INavigationService _navigation;

        [ObservableProperty]
        private ObservableCollection<Item> availableItems = new();

        [ObservableProperty]
        private ObservableCollection<Item> rentedOutItems = new();

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private bool isRefreshing;

        public MyItemsViewModel(
            IApiService apiService,
            IRentalService rentalService,
            IAuthenticationService authService,
            INavigationService navigation)
        {
            _apiService = apiService;
            _rentalService = rentalService;
            _authService = authService;
            _navigation = navigation;
        }

        [RelayCommand]
        public async Task LoadMyItemsAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                IsRefreshing = true;

                var userId = _authService.CurrentUserId;

                // 1️⃣ Load items you currently own
                var allItems = await _apiService.GetItemsAsync();
                var ownedItems = allItems.Where(i => i.OwnerId == userId).ToList();

                // 2️⃣ Load rentals for items you own (even when rented)
                var incomingRentals = await _rentalService.GetIncomingRentalsAsync();

                // Clear lists
                AvailableItems.Clear();
                RentedOutItems.Clear();

                // 3️⃣ Add available items
                foreach (var item in ownedItems)
                {
                    AvailableItems.Add(item);
                }

                // 4️⃣ Add rented-out items
                foreach (var rental in incomingRentals)
                {
                    var rentedItem = new Item
                    {
                        Id = rental.ItemId,
                        Title = rental.ItemTitle ?? "Unknown Item",
                        Description = rental.ItemDescription ?? "",
                        Category = "", // API doesn't return category in rental
                        OwnerId = rental.OwnerId,
                        OwnerName = rental.OwnerName ?? "",
                        IsAvailable = false,
                        // Rental info
                        CreatedBy = rental.OwnerId,
                        DailyRate = rental.TotalPrice,
                        // Custom rental fields
                        // You can add these to Item.cs if you want to display them
                    };

                    RentedOutItems.Add(rentedItem);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load items: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        private async Task ViewRequestsAsync(int itemId)
        {
            await _navigation.NavigateToAsync("rentalrequests", new Dictionary<string, object>
            {
                { "ItemId", itemId }
            });
        }
    }
}
