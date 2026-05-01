using RentalApp.ViewModels;

namespace RentalApp.Views;

public partial class NearbyItemsPage : ContentPage
{
    private readonly NearbyItemsViewModel _viewModel;

    public NearbyItemsPage(NearbyItemsViewModel vm)
    {
        InitializeComponent();
        BindingContext = _viewModel = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadNearbyItemsAsync();
    }
}
