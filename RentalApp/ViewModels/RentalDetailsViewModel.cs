using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RentalApp.Models;
using RentalApp.Services;

namespace RentalApp.ViewModels;

[QueryProperty(nameof(Rental), "Rental")]
public partial class RentalDetailsViewModel : ObservableObject
{
    private readonly IRentalService _rentalService;
    private readonly DatabaseService _databaseService;
    private readonly IApiService _api;

    public RentalDetailsViewModel(
        IRentalService rentalService,
        DatabaseService databaseService,
        IApiService api)
    {
        _rentalService = rentalService;
        _databaseService = databaseService;
        _api = api;
    }

    [ObservableProperty]
    private LocalRental rental;  

    // status checks
    public bool IsApproved => Rental?.Status?.Equals("Approved", StringComparison.OrdinalIgnoreCase) == true;
    public bool IsOutForRent => Rental?.Status?.Equals("Out for Rent", StringComparison.OrdinalIgnoreCase) == true;
    public bool IsReturned => Rental?.Status?.Equals("Returned", StringComparison.OrdinalIgnoreCase) == true;

    partial void OnRentalChanged(LocalRental value)
    {
        Console.WriteLine("DEBUG → Rental loaded:");
        Console.WriteLine($"DEBUG → Rental.Id = {value.ApiRentalId}");
        Console.WriteLine($"DEBUG → Rental.ItemId = {value.ApiItemId}");
        Console.WriteLine($"DEBUG → Rental.Status = {value.Status}");

        OnPropertyChanged(nameof(IsApproved));
        OnPropertyChanged(nameof(IsOutForRent));
        OnPropertyChanged(nameof(IsReturned));
    }

    [RelayCommand]
    public async Task MarkOutForRentAsync() => await UpdateStatus("Out for Rent");

    [RelayCommand]
    public async Task MarkReturnedAsync() => await UpdateStatus("Returned");

    [RelayCommand]
    public async Task MarkCompletedAsync()
    {
         await UpdateStatus("Completed");

        // ⭐ NEW: Set item back to available
        await _api.UpdateItemAsync(Rental.ApiItemId, new UpdateItemRequest
        {
            IsAvailable = true
        });

        // ⭐ Update local SQLite item
        var item = await _databaseService.GetItemByApiIdAsync(Rental.ApiItemId);
        if (item != null)
        {
            item.IsAvailable = true;
            item.LastSynced = DateTime.UtcNow;
            await _databaseService.SaveItemAsync(item);
        }

        await Shell.Current.DisplayAlert("Success", "Item is now available again.", "OK");

    } 

    private async Task UpdateStatus(string newStatus)
    {
        var success = await _rentalService.UpdateRentalStatusAsync(Rental.ApiRentalId, newStatus);

        if (!success)
        {
            await Shell.Current.DisplayAlert("Error", "Failed to update rental status.", "OK");
            return;
        }

        // Update local SQLite
        Rental.Status = newStatus;
        Rental.LastSynced = DateTime.UtcNow;
        await _databaseService.SaveRentalAsync(Rental);

        // Refresh UI
        OnPropertyChanged(nameof(IsApproved));
        OnPropertyChanged(nameof(IsOutForRent));
        OnPropertyChanged(nameof(IsReturned));

        await Shell.Current.DisplayAlert("Success", $"Rental marked as {newStatus}.", "OK");
    }
}
