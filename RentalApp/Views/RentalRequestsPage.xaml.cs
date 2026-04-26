using RentalApp.ViewModels;

namespace RentalApp.Views;

public partial class RentalRequestsPage : ContentPage
{
    public RentalRequestsPage(RentalRequestsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is RentalRequestsViewModel vm)
            await vm.LoadRequestsAsync();
    }
}
