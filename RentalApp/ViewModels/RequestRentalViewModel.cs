/*using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RentalApp.Database.Models;
using RentalApp.Repositories;
using RentalApp.Services;

namespace RentalApp.ViewModels;

[QueryProperty(nameof(Item), "Item")]
public partial class RequestRentalViewModel : BaseViewModel
{
    private readonly IRentalRepository _rentals;
    private readonly IAuthenticationService _auth;

    [ObservableProperty]
    private Item? item;

    [ObservableProperty]
    private DateTime startDate = DateTime.Today;

    [ObservableProperty]
    private DateTime endDate = DateTime.Today.AddDays(1);

    [ObservableProperty]
    private int numberOfDays;

    [ObservableProperty]
    private decimal totalCost;

    public RequestRentalViewModel(
        IRentalRepository rentals,
        IAuthenticationService auth)
    {
        _rentals = rentals;
        _auth = auth;

        Title = "Request Rental";

        Recalculate();
    }

    partial void OnStartDateChanged(DateTime value) => Recalculate();
    partial void OnEndDateChanged(DateTime value) => Recalculate();

    private void Recalculate()
    {
        if (Item == null)
        {
            NumberOfDays = 0;
            TotalCost = 0;
            return;
        }

        if (EndDate <= StartDate)
        {
            NumberOfDays = 0;
            TotalCost = 0;
            return;
        }

        NumberOfDays = (EndDate - StartDate).Days;
        TotalCost = NumberOfDays * Item.DailyRate;
    }

    [RelayCommand]
    private async Task SubmitRentalAsync()
    {
        System.Diagnostics.Debug.WriteLine($"SubmitRentalAsync fired. IsBusy={IsBusy}, Item null? {Item == null}");

        if (IsBusy || Item == null)
            return;

        if (NumberOfDays <= 0)
        {
            SetError("End date must be after start date.");
            return;
        }

        try
        {
            IsBusy = true;
            ClearError();

            var user = _auth.CurrentUser;
            if (user == null)
            {
                SetError("You must be logged in to request a rental.");
                return;
            }

            var request = new RentalRequest
            {
                itemId = Item.Id,
                startDate = DateOnly.FromDateTime(StartDate),
                endDate = DateOnly.FromDateTime(EndDate)
            };

            var result = await _rentals.RequestRentalAsync(request);

            if (result != null)
            {
                await Shell.Current.DisplayAlert("Success", "Rental request submitted!", "OK");
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                SetError("Failed to submit rental request.");
            }
        }
        catch (Exception ex)
        {
            SetError($"Rental request failed: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    partial void OnItemChanged(Item value)
    {
        Recalculate();
        System.Diagnostics.Debug.WriteLine($"Item received in RequestRentalViewModel: {value.Title}");
    }
}
*/