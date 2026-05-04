using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RentalApp.Database.Models;
using RentalApp.Repositories;
using RentalApp.Services;
using System.Collections.ObjectModel;

namespace RentalApp.ViewModels;

[QueryProperty(nameof(ItemId), "ItemId")]
public partial class RentalRequestsViewModel : BaseViewModel
{
    private readonly IRentalRepository _rentals;
    private readonly IAuthenticationService _auth;

    [ObservableProperty]
    private int itemId;

    [ObservableProperty]
    private ObservableCollection<Rental> incomingRequests = new();

    public RentalRequestsViewModel(
        IRentalRepository rentals,
        IAuthenticationService auth)
    {
        _rentals = rentals;
        _auth = auth;

        Title = "Rental Requests";
    }

    [RelayCommand]
    public async Task LoadRequestsAsync()
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

            // Load all incoming rentals for items I own
            var incoming = await _rentals.GetOwnerIncomingAsync(userId);

            //FILTER BY ITEM ID 
            var requests = incoming
                .Where(r =>
                    r.OwnerId == userId &&
                    r.ItemId == ItemId &&
                    (r.Status?.ToLower() == "pending" ||
                     r.Status?.ToLower() == "requested"))
                .ToList();

            IncomingRequests.Clear();
            foreach (var r in requests)
                IncomingRequests.Add(r);
        }
        catch (Exception ex)
        {
            SetError($"Failed to load rental requests: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task ApproveAsync(Rental rental)
    {
        if (rental == null)
            return;

        try
        {
            await _rentals.UpdateRentalStatusAsync(rental.Id, "approved");
            await LoadRequestsAsync();
        }
        catch (Exception ex)
        {
            SetError($"Failed to approve request: {ex.Message}");
        }
    }

    [RelayCommand]
    public async Task RejectAsync(Rental rental)
    {
        if (rental == null)
            return;

        try
        {
            await _rentals.UpdateRentalStatusAsync(rental.Id, "rejected");
            await LoadRequestsAsync();
        }
        catch (Exception ex)
        {
            SetError($"Failed to reject request: {ex.Message}");
        }
    }
}
