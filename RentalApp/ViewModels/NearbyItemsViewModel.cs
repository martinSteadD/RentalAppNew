using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RentalApp.Database.Models;
using RentalApp.Services;
using System.Collections.ObjectModel;

namespace RentalApp.ViewModels
{
    public partial class NearbyItemsViewModel : BaseViewModel
    {
        private readonly IApiService _apiService;
        private readonly ILocationService _locationService;

        [ObservableProperty]
        private ObservableCollection<Item> items = new();

        public NearbyItemsViewModel(IApiService apiService, ILocationService locationService)
        {
            _apiService = apiService;
            _locationService = locationService;

            Title = "Nearby Items";
        }

        [RelayCommand]
        public async Task LoadNearbyItemsAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                // 1. Get device location (tuple deconstruction)
                var (latitude, longitude) = await _locationService.GetCurrentLocationAsync();

                Console.WriteLine($"User location: {latitude}, {longitude}");

                // 2. Call API using the tuple values
                var results = await _apiService.GetNearbyItemsAsync(latitude, longitude, 50);

                // 3. Update UI
                Items.Clear();

                Console.WriteLine($"Items returned: {results.Count()}");

                foreach (var item in results)
                    Items.Add(item);
            }
            catch (Exception ex)
            {
                SetError($"Failed to load nearby items: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task RefreshAsync()
        {
            await LoadNearbyItemsAsync();
        }
    }
}
