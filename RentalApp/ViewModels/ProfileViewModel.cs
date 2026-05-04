using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RentalApp.Database.Models;
using RentalApp.Services;

namespace RentalApp.ViewModels;

public partial class ProfileViewModel : BaseViewModel
{
    private readonly IAuthenticationService _authService;

    [ObservableProperty]
    private ApiUser? currentUser;

    [ObservableProperty]
    private string currentPassword = string.Empty;

    [ObservableProperty]
    private string newPassword = string.Empty;

    [ObservableProperty]
    private string confirmNewPassword = string.Empty;

    [ObservableProperty]
    private bool isChangingPassword;

    public ProfileViewModel(IAuthenticationService authService)
    {
        _authService = authService;
        Title = "Profile";

        LoadUserData();
    }

    private void LoadUserData()
    {
        CurrentUser = _authService.CurrentUser;
    }

    // ---------------------------------------------------------
    // COMPUTED PROPERTIES
    // ---------------------------------------------------------

    public string FullName =>
        CurrentUser == null
            ? ""
            : $"{CurrentUser.FirstName} {CurrentUser.LastName}";

    public string JoinDateFormatted =>
        CurrentUser == null
            ? ""
            : $"Member since {CurrentUser.CreatedAt:dd MMM yyyy}";

    public string DisplayRating =>
        CurrentUser?.AverageRating == null
            ? "Rating: N/A"
            : $"Rating: {CurrentUser.AverageRating:F1} ★";

    public string ItemsListedFormatted =>
        CurrentUser == null
            ? "Items Listed: 0"
            : $"Items Listed: {CurrentUser.ItemsListed}";

    public string RentalsCompletedFormatted =>
        CurrentUser == null
            ? "Rentals Completed: 0"
            : $"Rentals Completed: {CurrentUser.RentalsCompleted}";

    partial void OnCurrentUserChanged(ApiUser? value)
    {
        OnPropertyChanged(nameof(FullName));
        OnPropertyChanged(nameof(JoinDateFormatted));
        OnPropertyChanged(nameof(DisplayRating));
        OnPropertyChanged(nameof(ItemsListedFormatted));
        OnPropertyChanged(nameof(RentalsCompletedFormatted));
    }

    // ---------------------------------------------------------
    // PASSWORD CHANGE (disabled)
    // ---------------------------------------------------------

    [RelayCommand]
    private async Task ChangePasswordAsync()
    {
        if (IsBusy)
            return;

        if (!ValidatePasswordChange())
            return;

        try
        {
            IsBusy = true;
            ClearError();

            await Shell.Current.DisplayAlertAsync(
                "Not Available",
                "Password change is not supported in this version of the app.",
                "OK");

            ClearPasswordFields();
            IsChangingPassword = false;
        }
        catch (Exception ex)
        {
            SetError($"Password change failed: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void TogglePasswordChangeMode()
    {
        IsChangingPassword = !IsChangingPassword;

        if (!IsChangingPassword)
        {
            ClearPasswordFields();
            ClearError();
        }
    }

    [RelayCommand]
    private async Task NavigateBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    private bool ValidatePasswordChange()
    {
        if (string.IsNullOrWhiteSpace(CurrentPassword))
        {
            SetError("Current password is required");
            return false;
        }

        if (string.IsNullOrWhiteSpace(NewPassword))
        {
            SetError("New password is required");
            return false;
        }

        if (NewPassword.Length < 6)
        {
            SetError("New password must be at least 6 characters long");
            return false;
        }

        if (NewPassword != ConfirmNewPassword)
        {
            SetError("New passwords do not match");
            return false;
        }

        if (CurrentPassword == NewPassword)
        {
            SetError("New password must be different from current password");
            return false;
        }

        return true;
    }

    private void ClearPasswordFields()
    {
        CurrentPassword = string.Empty;
        NewPassword = string.Empty;
        ConfirmNewPassword = string.Empty;
    }
}
