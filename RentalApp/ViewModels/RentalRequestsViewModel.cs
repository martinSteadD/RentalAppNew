using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RentalApp.Database.Models;
using RentalApp.Services;
using RentalApp.Models;
using System.Collections.ObjectModel;

namespace RentalApp.ViewModels;

public partial class RentalRequestsViewModel : BaseViewModel
{
    private readonly IRentalService _rentalService;
    private readonly DatabaseService _databaseService;
    private readonly IAuthenticationService _authService;

    [ObservableProperty]
    private ObservableCollection<LocalRental> requests = new();

    public RentalRequestsViewModel(
        IRentalService rentalService,
        DatabaseService databaseService,
        IAuthenticationService authService)
    {
        _rentalService = rentalService;
        _databaseService = databaseService;
        _authService = authService;
    }

    public async Task LoadRequestsAsync()
    {
        try
        {
            IsBusy = true;
            HasError = false;

            int ownerId = _authService.CurrentUser!.Id;

            // Pull incoming rentals from API
            var apiRentals = await _rentalService.GetIncomingRentalsAsync();

            // Save API rentals into SQLite
            foreach (var rental in apiRentals)
            {
                var local = new LocalRental
                {
                    ApiRentalId = rental.Id,
                    ApiItemId = rental.ItemId,
                    BorrowerId = rental.BorrowerId,
                    RequestedBy = rental.BorrowerId,
                    Status = rental.Status,
                    LastSynced = DateTime.UtcNow
                };

                await _databaseService.SaveRentalAsync(local);
            }

            // Load all rentals from SQLite
            var allRentals = await _databaseService.GetAllRentalsAsync();

            // Get all items owned by this user
            var myItems = await _databaseService.GetAllItemsAsync();
            var myItemIds = myItems
                .Where(i => i.CreatedBy == ownerId)
                .Select(i => i.ApiItemId)
                .ToList();

            // Filter rentals for items I own + pending/requested
            var pending = allRentals
                .Where(r => myItemIds.Contains(r.ApiItemId))
                .Where(r =>
                {
                    var status = r.Status?.Trim().ToLower() ?? "";
                    return status == "pending" || status == "requested";
                })
                .ToList();

            Requests = new ObservableCollection<LocalRental>(pending);
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
    public async Task ApproveAsync(int rentalId)
    {
        try
        {
            var success = await _rentalService.UpdateRentalStatusAsync(rentalId, "Approved");

            if (!success)
            {
                SetError("Failed to approve request.");
                return;
            }

            var rental = await _databaseService.GetRentalByApiIdAsync(rentalId);
            if (rental != null)
            {
                rental.Status = "Approved";
                rental.LastSynced = DateTime.UtcNow;
                await _databaseService.SaveRentalAsync(rental);
            }

            await LoadRequestsAsync();
        }
        catch (Exception ex)
        {
            SetError($"Failed to approve request: {ex.Message}");
        }
    }

    [RelayCommand]
    public async Task RejectAsync(int rentalId)
    {
        try
        {
            var success = await _rentalService.UpdateRentalStatusAsync(rentalId, "Rejected");

            if (!success)
            {
                SetError("Failed to reject request.");
                return;
            }

            var rental = await _databaseService.GetRentalByApiIdAsync(rentalId);
            if (rental != null)
            {
                rental.Status = "Rejected";
                rental.LastSynced = DateTime.UtcNow;
                await _databaseService.SaveRentalAsync(rental);
            }

            await LoadRequestsAsync();
        }
        catch (Exception ex)
        {
            SetError($"Failed to reject request: {ex.Message}");
        }
    }
}
