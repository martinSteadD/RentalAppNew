using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RentalApp.Database.Models;
using RentalApp.Services;
using System.Collections.ObjectModel;

namespace RentalApp.ViewModels;

[QueryProperty(nameof(Item), "Item")]
public partial class ItemDetailsViewModel : BaseViewModel
{
    private readonly IRentalService _rentalService;
    private readonly INavigationService _navigationService;
    private readonly IReviewService _reviewService;

    [ObservableProperty]
    private Item? item;

    public ObservableCollection<Review> Reviews { get; } = new();

    // ⭐ New backing property for API-provided average rating
    [ObservableProperty]
    private double averageRating;

    public string AvailabilityText => "Available";

    public Color AvailabilityColor => Colors.Green;

    public ItemDetailsViewModel(
        IRentalService rentalService,
        INavigationService navigationService,
        IReviewService reviewService)
    {
        _rentalService = rentalService;
        _navigationService = navigationService;
        _reviewService = reviewService;

        Title = "Item Details";
    }

    // Called by the page when navigated to
    public async Task LoadAsync()
    {
        if (Item == null)
            return;

        await LoadReviewsAsync();
    }

    private async Task LoadReviewsAsync()
    {
        try
        {
            // Get the full wrapper response (reviews + averageRating + pagination)
            var response = await _reviewService.GetReviewsForItemAsync(Item.Id);

            if (response == null)
            {
                SetError("Failed to load reviews: response was null");
                return;
            }

            // Sort newest first
            var sorted = response.Reviews
                .OrderByDescending(r => r.CreatedAt)
                .ToList();

            // Update the ObservableCollection
            Reviews.Clear();
            foreach (var r in sorted)
                Reviews.Add(r);

            // Update the AverageRating property from the API
            AverageRating = response.AverageRating;
        }
        catch (Exception ex)
        {
            SetError($"Failed to load reviews: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task RefreshReviewsAsync()
    {
        await LoadReviewsAsync();
    }

    [RelayCommand]
    private async Task RequestRentalAsync()
    {
        if (IsBusy || Item == null)
            return;

        try
        {
            IsBusy = true;
            ClearError();

            var result = await _rentalService.RequestRentalAsync(Item.Id);

            if (result != null)
            {
                await Shell.Current.DisplayAlertAsync(
                    "Success",
                    "Rental request submitted!",
                    "OK");
            }
            else
            {
                SetError("Failed to request rental.");
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

    [RelayCommand]
    private async Task NavigateBackAsync()
    {
        await _navigationService.NavigateBackAsync();
    }
}
