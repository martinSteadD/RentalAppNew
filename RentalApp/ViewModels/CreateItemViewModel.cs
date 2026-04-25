using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RentalApp.Database.Models;
using RentalApp.Services;
using System.Collections.ObjectModel;

namespace RentalApp.ViewModels;

public partial class CreateItemViewModel : BaseViewModel
{
    private readonly IApiService _apiService;
    private readonly IAuthenticationService _authService;
    private readonly INavigationService _navigationService;

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
        IApiService apiService,
        IAuthenticationService authService,
        INavigationService navigationService)
    {
        _apiService = apiService;
        _authService = authService;
        _navigationService = navigationService;

        Title = "Create Item";

        _ = LoadCategoriesAsync();
    }

    private async Task LoadCategoriesAsync()
    {
        try
        {
            var list = await _apiService.GetCategoriesAsync();

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

            var item = new CreateItemRequest
            {
                Title = TitleText,
                Description = Description,
                DailyRate = decimal.Parse(DailyRate),
                CategoryId = SelectedCategory.Id,
                Latitude = 0,
                Longitude = 0
            };

            var result = await _apiService.CreateItemAsync(item);

            if (result == null)
            {
                SetError("Failed to create item.");
                return;
            }

            await Shell.Current.DisplayAlertAsync("Success", "Item created successfully!", "OK");
            await _navigationService.NavigateBackAsync();
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
