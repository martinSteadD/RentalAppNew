using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RentalApp.Services;

namespace RentalApp.ViewModels;

public partial class AppShellViewModel : ObservableObject
{
    private readonly IAuthenticationService _authService;

    public AppShellViewModel(IAuthenticationService authService)
    {
        _authService = authService;
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
    private async Task NavigateToMyRentalsAsync()
    {
        await Shell.Current.GoToAsync("myrentals");
    }

    [RelayCommand]
    private async Task NavigateToBrowseItemsAsync()
    {
        await Shell.Current.GoToAsync("browse");
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        await _authService.LogoutAsync();
        await Shell.Current.GoToAsync("//login");
    }
}
