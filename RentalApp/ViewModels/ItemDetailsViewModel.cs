using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RentalApp.Database.Models;
using RentalApp.Repositories;
using RentalApp.Services;
using System.Collections.ObjectModel;

namespace RentalApp.ViewModels;

[QueryProperty(nameof(Item), "Item")]
public partial class ItemDetailsViewModel : BaseViewModel
{
    private readonly IRentalRepository _rentals;
    private readonly IReviewService _reviews;
    private readonly IAuthenticationService _auth;

    [ObservableProperty]
    private Item? item;

    [ObservableProperty]
    private bool isOwner;

    public ObservableCollection<Review> Reviews { get; } = new();

    [ObservableProperty]
    private double averageRating;

    public string AvailabilityText => "Available";
    public Color AvailabilityColor => Colors.Green;

    public ItemDetailsViewModel(
        IRentalRepository rentals,
        IReviewService reviews,
        IAuthenticationService auth)
    {
        _rentals = rentals;
        _reviews = reviews;
        _auth = auth;

        Title = "Item Details";
    }

    partial void OnItemChanged(Item? value)
    {
        if (value == null)
            return;

        var user = _auth.CurrentUser;
        IsOwner = user != null && value.OwnerId == user.Id;
    }

    public async Task LoadAsync()
    {
        if (Item == null)
            return;

        var user = _auth.CurrentUser;
        IsOwner = user != null && Item.OwnerId == user.Id;

        await LoadReviewsAsync();
    }

    private async Task LoadReviewsAsync()
    {
        try
        {
            ClearError();

            if (Item == null)
                return;

            var response = await _reviews.GetReviewsForItemAsync(Item.Id);

            if (response == null)
            {
                SetError("Failed to load reviews.");
                return;
            }

            var sorted = response.Reviews
                .OrderByDescending(r => r.CreatedAt)
                .ToList();

            Reviews.Clear();
            foreach (var r in sorted)
                Reviews.Add(r);

            AverageRating = response.AverageRating ?? 0;
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

            await Shell.Current.GoToAsync("requestrental", true, new Dictionary<string, object>
            {
                { "Item", Item }
            });
        }
        catch (Exception ex)
        {
            SetError($"Navigation failed: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task EditItemAsync()
    {
        if (Item == null)
            return;

        await Shell.Current.GoToAsync("edititem", true, new Dictionary<string, object>
        {
            { "Item", Item }
        });
    }

    [RelayCommand]
    private async Task NavigateBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    public async Task SubmitRentalRequestAsync()
    {
        try
        {
            if (Item == null)
            {
                await Shell.Current.DisplayAlert("Error", "Item not found.", "OK");
                return;
            }

            var request = new RentalRequest
            {
                itemId = Item.Id,
                startDate = DateTime.Today.ToString("yyyy-MM-dd"),
                endDate = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd")
            };

            var result = await _rentals.RequestRentalAsync(request);

            if (result != null)
            {
                await Shell.Current.DisplayAlert("Success", "Rental request submitted.", "OK");
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "Failed to submit rental request.", "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Something went wrong: {ex.Message}", "OK");
        }
    }

}
