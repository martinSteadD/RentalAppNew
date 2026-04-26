using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RentalApp.Database.Models;
using RentalApp.Services;
using System.Collections.ObjectModel;

namespace RentalApp.ViewModels;

public partial class RentalRequestsViewModel : BaseViewModel
{
    private readonly IRentalService _rentalService;

    [ObservableProperty]
    private ObservableCollection<Rental> requests = new();

    private int _itemId;

    public RentalRequestsViewModel(IRentalService rentalService)
    {
        _rentalService = rentalService;
    }

    public void Initialize(int itemId)
    {
        _itemId = itemId;
    }

    public async Task LoadRequestsAsync()
    {
        try
        {
            IsBusy = true;
            HasError = false;

            // 1. Get all incoming rental requests (for items I own)
            var rentals = await _rentalService.GetIncomingRentalsAsync();

            // 2. Filter to only requests for THIS item and only Pending ones
            var pending = rentals
                .Where(r => r.ItemId == _itemId &&
                 (r.Status == "Requested" || r.Status == "Pending"));

            // 3. Update the UI
            Requests = new ObservableCollection<Rental>(pending);
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
            await _rentalService.ApproveRentalAsync(rentalId);
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
            await _rentalService.CancelRentalAsync(rentalId);
            await LoadRequestsAsync();
        }
        catch (Exception ex)
        {
            SetError($"Failed to reject request: {ex.Message}");
        }
    }
}
