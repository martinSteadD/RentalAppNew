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

        // User‑editable radius
        [ObservableProperty]
        private int radius = 50;

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
                ClearError();

                // 1. Get device location
                var (latitude, longitude) = await _locationService.GetCurrentLocationAsync();

                // 2. Call API using the user‑selected radius
                var results = await _apiService.GetNearbyItemsAsync(latitude, longitude, Radius);

                // 3. Update UI
                Items.Clear();

                foreach (var item in results.Take(30))
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
