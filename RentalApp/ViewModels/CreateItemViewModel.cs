using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RentalApp.Database.Models;
using RentalApp.Services;
using RentalApp.Repositories;
using System.Collections.ObjectModel;

namespace RentalApp.ViewModels;

public partial class CreateItemViewModel : BaseViewModel
{
    private readonly IItemRepository _items;
    private readonly IAuthenticationService _authService;
    private readonly ILocationService _locationService;

    [ObservableProperty]
    private string titleText = string.Empty;

    [ObservableProperty]
    private string description = string.Empty;

    [ObservableProperty]
    private string dailyRate = string.Empty;

    public ObservableCollection<Category> Categories { get; } = new();

    [ObservableProperty]
    private Category? selectedCategory;

    public CreateItemViewModel(
        IItemRepository items,
        IAuthenticationService authService,
        ILocationService locationService)
    {
        _items = items;
        _authService = authService;
        _locationService = locationService;

        Title = "Create Item";

        _ = LoadCategoriesAsync();
    }

    private async Task LoadCategoriesAsync()
    {
        try
        {
            var list = await _items.GetCategoriesAsync();

            Categories.Clear();
            foreach (var c in list)
                Categories.Add(c);
        }
        catch
        {
            await Shell.Current.DisplayAlertAsync("Error", "Failed to load categories.", "OK");
        }
    }

    [RelayCommand]
    private async Task CreateItemAsync()
    {
        if (IsBusy)
            return;

        if (string.IsNullOrWhiteSpace(TitleText) ||
            string.IsNullOrWhiteSpace(Description) ||
            string.IsNullOrWhiteSpace(DailyRate) ||
            SelectedCategory == null)
        {
            SetError("Please fill in all required fields.");
            return;
        }

        try
        {
            IsBusy = true;
            ClearError();

            var (lat, lng) = await _locationService.GetCurrentLocationAsync();

            var newItem = new Item
            {
                Title = TitleText,
                Description = Description,
                DailyRate = decimal.Parse(DailyRate),
                CategoryId = SelectedCategory.Id,
                Category = SelectedCategory.Name,
                Latitude = lat,
                Longitude = lng,
                OwnerId = _authService.CurrentUser.Id
            };

            var created = await _items.CreateAsync(newItem);

            if (created == null)
            {
                SetError("Failed to create item.");
                return;
            }

            await Shell.Current.DisplayAlertAsync("Success", "Item created successfully!", "OK");
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            SetError($"Failed to create item: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
