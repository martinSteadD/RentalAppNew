using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RentalApp.Database.Models;
using RentalApp.Repositories;
using RentalApp.Services;

namespace RentalApp.ViewModels;

[QueryProperty(nameof(Rental), "Rental")]
public partial class RentalDetailsViewModel : ObservableObject
{
    private readonly IRentalRepository _rentals;
    private readonly IAuthenticationService _auth;

    [ObservableProperty]
    private Rental? rental;

    [ObservableProperty]
    private bool hasError;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    public RentalDetailsViewModel(
        IRentalRepository rentals,
        IAuthenticationService auth)
    {
        _rentals = rentals;
        _auth = auth;
    }

    [RelayCommand]
    public async Task MarkOutForRentAsync()
    {
        if (Rental == null)
            return;

        try
        {
            await _rentals.UpdateRentalStatusAsync(Rental.Id, "out for rent");
            await Shell.Current.DisplayAlert("Success", "Marked as Out for Rent.", "OK");
        }
        catch (Exception ex)
        {
            HasError = true;
            ErrorMessage = $"Failed to update status: {ex.Message}";
        }
    }

    [RelayCommand]
    public async Task MarkReturnedAsync()
    {
        if (Rental == null)
            return;

        try
        {
            await _rentals.UpdateRentalStatusAsync(Rental.Id, "returned");
            await Shell.Current.DisplayAlert("Success", "Marked as Returned.", "OK");
        }
        catch (Exception ex)
        {
            HasError = true;
            ErrorMessage = $"Failed to update status: {ex.Message}";
        }
    }
}
