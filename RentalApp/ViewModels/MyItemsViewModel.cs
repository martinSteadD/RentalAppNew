using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RentalApp.Database.Models;
using RentalApp.Models;
using RentalApp.Repositories;
using RentalApp.Services;
using System.Collections.ObjectModel;

namespace RentalApp.ViewModels;

public partial class MyItemsViewModel : BaseViewModel
{
    private readonly IItemRepository _items;
    private readonly IRentalRepository _rentals;
    private readonly IAuthenticationService _auth;

    [ObservableProperty]
    private ObservableCollection<Item> availableItems = new();

    [ObservableProperty]
    private ObservableCollection<MyItemRentalDisplay> rentedOutItems = new();

    public MyItemsViewModel(
        IItemRepository items,
        IRentalRepository rentals,
        IAuthenticationService auth)
    {
        _items = items;
        _rentals = rentals;
        _auth = auth;

        Title = "My Items";
    }

    [RelayCommand]
    public async Task LoadItemsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            var user = _auth.CurrentUser;
            if (user == null)
                return;

            int userId = user.Id;

            var allItems = await _items.GetAllAsync();
            var myItems = allItems.Where(i => i.OwnerId == userId).ToList();

            var incomingRentals = await _rentals.GetOwnerIncomingAsync(userId);

            AvailableItems.Clear();
            RentedOutItems.Clear();
            Console.WriteLine($"🔍 MyItems: Found {myItems.Count} items for owner {userId}");

            foreach (var item in myItems)
            {
                var rental = incomingRentals
                    .Where(r => r.ItemId == item.Id)
                    .OrderByDescending(r => r.StartDate)
                    .FirstOrDefault();

                if (rental == null)
                {
                    AvailableItems.Add(item);
                    continue;
                }

                string status = rental.Status?.Trim() ?? "";

                if (status is "approved" or "out for rent" or "returned")
                {
                    RentedOutItems.Add(new MyItemRentalDisplay
                    {
                        Item = item,
                        Rental = rental
                    });
                }
                else
                {
                    AvailableItems.Add(item);
                }
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task OpenRentalDetailsAsync(MyItemRentalDisplay display)
    {
        if (display == null)
            return;

        await Shell.Current.GoToAsync("rentaldetails", new Dictionary<string, object>
        {
            { "Rental", display.Rental },
            { "Item", display.Item }
        });
    }

    [RelayCommand]
    public async Task ViewRentalRequestsAsync(Item item)
    {
        if (item == null)
            return;

        //  Pass ItemId, not the whole Item
        await Shell.Current.GoToAsync($"rentalrequests?ItemId={item.Id}");
    }
}
