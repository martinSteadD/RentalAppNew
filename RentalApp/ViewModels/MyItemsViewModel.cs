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
        private readonly IAuthenticationService _authService;
        private readonly INavigationService _navigation;

        [ObservableProperty]
        private ObservableCollection<Item> myItems = new();

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private bool isRefreshing;

        public MyItemsViewModel(
            IApiService apiService,
            IAuthenticationService authService,
            INavigationService navigation)
        {
            _apiService = apiService;
            _authService = authService;
            _navigation = navigation;
        }

        // Called when page appears or user pulls to refresh
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

                // Fetch all items from API
                var items = await _apiService.GetItemsAsync();

                // Filter to only items owned by the logged-in user
                MyItems = new ObservableCollection<Item>(
                    items.Where(i => i.OwnerId == userId));
            }
            catch (Exception ex)
            {
                // Optional: Add error handling UI later
                Console.WriteLine($"Failed to load items: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
                IsRefreshing = false;
            }
        }

        // Navigate to rental requests for a specific item
        [RelayCommand]
        private async Task ViewRequestsAsync(int itemId)
        {
            await _navigation.NavigateToAsync($"rentalrequests?itemId={itemId}");
        }
    }
}
