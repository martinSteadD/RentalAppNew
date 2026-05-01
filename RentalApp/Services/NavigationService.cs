using RentalApp.Views;
using RentalApp.ViewModels;
using RentalApp.Database.Models;

namespace RentalApp.Services;

public class NavigationService : INavigationService
{
    private readonly IServiceProvider _serviceProvider;

    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task NavigateToAsync(string route)
    {
        Page page = route switch
        {
            "login" => _serviceProvider.GetRequiredService<LoginPage>(),
            "register" => _serviceProvider.GetRequiredService<RegisterPage>(),
            "browse" => _serviceProvider.GetRequiredService<BrowseItemsPage>(),
            "itemdetails" => _serviceProvider.GetRequiredService<ItemDetailsPage>(),
            "createitem" => _serviceProvider.GetRequiredService<CreateItemPage>(),
            "myrentals" => _serviceProvider.GetRequiredService<MyRentalsPage>(),
            "profile" => _serviceProvider.GetRequiredService<ProfilePage>(),
            "userlist" => _serviceProvider.GetRequiredService<UserListPage>(),
            "main" => _serviceProvider.GetRequiredService<MainPage>(),
            "myItems" => _serviceProvider.GetRequiredService<MyItemsPage>(),
            "rentalrequests" => _serviceProvider.GetRequiredService<RentalRequestsPage>(),
            "rentaldetails" => _serviceProvider.GetRequiredService<RentalDetailsPage>(),
            "nearbyitems" => _serviceProvider.GetRequiredService<NearbyItemsPage>(),
            _ => throw new Exception($"Unknown route: {route}")
        };

        await Shell.Current.Navigation.PushAsync(page);
    }

    public async Task NavigateToAsync(string route, Dictionary<string, object> parameters)
    {
        Page page = route switch
        {
            "itemdetails" => _serviceProvider.GetRequiredService<ItemDetailsPage>(),
            "rentalrequests" => _serviceProvider.GetRequiredService<RentalRequestsPage>(),
            _ => throw new Exception($"Unknown route: {route}")
        };

        // Pass Item into ItemDetailsViewModel
        if (page.BindingContext is ItemDetailsViewModel itemVm &&
            parameters.TryGetValue("Item", out var itemObj) &&
            itemObj is Item item)
        {
            itemVm.Item = item;
        }
        await Shell.Current.Navigation.PushAsync(page);
    }

    public async Task NavigateBackAsync()
    {
        await Shell.Current.Navigation.PopAsync();
    }

    public async Task NavigateToRootAsync()
    {
        await Shell.Current.Navigation.PopToRootAsync();
    }

    public async Task PopToRootAsync()
    {
        await Shell.Current.Navigation.PopToRootAsync();
    }
}
