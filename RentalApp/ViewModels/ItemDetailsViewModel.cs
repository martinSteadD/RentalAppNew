using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RentalApp.Database.Models;
using RentalApp.Services;

namespace RentalApp.ViewModels;

[QueryProperty(nameof(Item), "Item")]
public partial class ItemDetailsViewModel : BaseViewModel
{
    private readonly IRentalService _rentalService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private Item? item;

    public ItemDetailsViewModel(IRentalService rentalService, INavigationService navigationService)
    {
        _rentalService = rentalService;
        _navigationService = navigationService;
        Title = "Item Details";
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
