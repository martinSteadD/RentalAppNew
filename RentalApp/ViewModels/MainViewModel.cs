using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RentalApp.Database.Models;
using RentalApp.Services;

namespace RentalApp.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    private readonly IAuthenticationService _authService;

    [ObservableProperty]
    private ApiUser? currentUser;

    [ObservableProperty]
    private string welcomeMessage = string.Empty;

    public MainViewModel(IAuthenticationService authService)
    {
        _authService = authService;
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
            await Shell.Current.GoToAsync("//login");
        }
    }

    [RelayCommand]
    private async Task NavigateToProfileAsync()
    {
        await Shell.Current.GoToAsync("profile");
    }

    [RelayCommand]
    private async Task NavigateToSettingsAsync()
    {
        await Shell.Current.GoToAsync("settings");
    }

    [RelayCommand]
    private async Task NavigateToBrowseItemsAsync()
    {
        await Shell.Current.GoToAsync("browse");
    }

    [RelayCommand]
    private async Task NavigateToMyRentalsAsync()
    {
        await Shell.Current.GoToAsync("myrentals");
    }

    [RelayCommand]
    private async Task NavigateToCreateItemAsync()
    {
        await Shell.Current.GoToAsync("createitem");
    }

    [RelayCommand]
    private async Task NavigateToMyItemsAsync()
    {
        await Shell.Current.GoToAsync("myitems");
    }

    [RelayCommand]
    private async Task NavigateToNearbyItemsAsync()
    {
        await Shell.Current.GoToAsync("nearbyitems");
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
