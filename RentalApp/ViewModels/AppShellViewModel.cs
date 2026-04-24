using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RentalApp.Services;

namespace RentalApp.ViewModels;

public partial class AppShellViewModel : ObservableObject
{
    private readonly IAuthenticationService _authService;
    private readonly INavigationService _navigationService;

    public AppShellViewModel(IAuthenticationService authService, INavigationService navigationService)
    {
        _authService = authService;
        _navigationService = navigationService;
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
    private async Task NavigateToMyRentalsAsync()
    {
        await _navigationService.NavigateToAsync("myrentals");
    }

    [RelayCommand]
    private async Task NavigateToBrowseItemsAsync()
    {
        await _navigationService.NavigateToAsync("browse");
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        await _authService.LogoutAsync();
        await _navigationService.NavigateToAsync("//login");
    }
}
