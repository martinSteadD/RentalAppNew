using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RentalApp.Database.Models;
using RentalApp.Services;

namespace RentalApp.ViewModels;

[QueryProperty(nameof(RentalId), "RentalId")]
public partial class AddReviewViewModel : BaseViewModel
{
    private readonly IReviewService _reviewService;

    [ObservableProperty]
    private int rentalId;

    [ObservableProperty]
    private int rating;

    [ObservableProperty]
    private string comment = string.Empty;

    public List<int> Ratings { get; } = new() { 1, 2, 3, 4, 5 };

    public AddReviewViewModel(IReviewService reviewService)
    {
        _reviewService = reviewService;
        Title = "Add Review";
    }

    [RelayCommand]
    private async Task SubmitAsync()
    {
        if (IsBusy)
            return;

        if (Rating < 1 || Rating > 5)
        {
            SetError("Please select a rating between 1 and 5.");
            return;
        }

        try
        {
            IsBusy = true;
            ClearError();

            var request = new CreateReviewRequest
            {
                RentalId = RentalId,
                Rating = Rating,
                Comment = Comment
            };

            var success = await _reviewService.SubmitReviewAsync(request);

            if (success)
            {
                await Shell.Current.DisplayAlertAsync(
                    "Success",
                    "Your review has been submitted!",
                    "OK");

                await Shell.Current.GoToAsync(".."); // navigate back
            }
            else
            {
                SetError("Failed to submit review.");
            }
        }
        catch (Exception ex)
        {
            SetError($"Error submitting review: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task CancelAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
