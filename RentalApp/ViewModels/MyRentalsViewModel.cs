using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RentalApp.Database.Models;
using RentalApp.Repositories;
using RentalApp.Services;
using System.Collections.ObjectModel;

namespace RentalApp.ViewModels;

public partial class MyRentalsViewModel : BaseViewModel
{
    private readonly IRentalRepository _rentals;
    private readonly IAuthenticationService _auth;

    [ObservableProperty]
    private ObservableCollection<Rental> myRentals = new();

    public MyRentalsViewModel(
        IRentalRepository rentals,
        IAuthenticationService auth)
    {
        _rentals = rentals;
        _auth = auth;

        Title = "My Rentals";
    }

    [RelayCommand]
    public async Task LoadRentalsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            ClearError();

            var user = _auth.CurrentUser;
            if (user == null)
                return;

            int userId = user.Id;

            // Rentals where I am the borrower
            var outgoing = await _rentals.GetOutgoingRentalsAsync();
            var mine = outgoing
                .Where(r => r.BorrowerId == userId)
                .OrderByDescending(r => r.StartDate)
                .ToList();

            MyRentals.Clear();
            foreach (var rental in mine)
                MyRentals.Add(rental);
        }
        catch (Exception ex)
        {
            SetError($"Failed to load rentals: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task OpenRentalDetailsAsync(Rental rental)
    {
        if (rental == null)
            return;

        try
        {
            await Shell.Current.GoToAsync("rentaldetails", new Dictionary<string, object>
            {
                { "Rental", rental }
            });
        }
        catch (Exception ex)
        {
            SetError($"Failed to open rental details: {ex.Message}");
        }
    }
}
