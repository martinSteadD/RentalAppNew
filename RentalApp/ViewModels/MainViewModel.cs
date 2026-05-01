using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RentalApp.Database.Models;
using RentalApp.Services;

namespace RentalApp.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    private readonly IAuthenticationService _authService;
    private readonly INavigationService _navigationService;

    // FIX: Use ApiUser instead of User
    [ObservableProperty]
    private ApiUser? currentUser;

    [ObservableProperty]
    private string welcomeMessage = string.Empty;

    public MainViewModel()
    {
        Title = "Dashboard";
    }

    public MainViewModel(IAuthenticationService authService, INavigationService navigationService)
    {
        _authService = authService;
        _navigationService = navigationService;
        Title = "Dashboard";

        LoadUserData();
    }

    private void LoadUserData()
    {
        CurrentUser = _authService.CurrentUser;

        if (CurrentUser != null)
        {
            WelcomeMessage = $"Welcome, {CurrentUser.FirstName} {CurrentUser.LastName}!";
        }
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        var result = await Shell.Current.DisplayAlertAsync(
            "Logout",
            "Are you sure you want to logout?",
            "Yes",
            "No");

        if (result)
        {
            await _authService.LogoutAsync();
            await _navigationService.NavigateToAsync("login");
        }
    }

    [RelayCommand]
    private async Task NavigateToProfileAsync()
    {
        await _navigationService.NavigateToAsync("profile");
    }

    [RelayCommand]
    private async Task NavigateToSettingsAsync()
    {
        await _navigationService.NavigateToAsync("settings");
    }

    [RelayCommand]
    private async Task NavigateToBrowseItemsAsync()
    {
        await _navigationService.NavigateToAsync("browse");
    }

    [RelayCommand]
    private async Task NavigateToMyRentalsAsync()
    {
        await _navigationService.NavigateToAsync("myrentals");
    }

    [RelayCommand]
    private async Task NavigateToCreateItemAsync()
    {
        await _navigationService.NavigateToAsync("createitem");
    }

    [RelayCommand]
    private async Task NavigateToMyItemsAsync()
    {
        await _navigationService.NavigateToAsync("myItems");
    }

    [RelayCommand]
    private async Task NavigateToNearbyItemsAsync()
    {
        await _navigationService.NavigateToAsync("nearbyitems");
    }


    [RelayCommand]
    private async Task RefreshDataAsync()
    {
        try
        {
            IsBusy = true;
            LoadUserData();
            await Task.Delay(500);
        }
        catch (Exception ex)
        {
            SetError($"Failed to refresh data: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
