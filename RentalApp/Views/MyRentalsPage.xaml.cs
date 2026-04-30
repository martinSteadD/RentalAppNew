using RentalApp.ViewModels;

namespace RentalApp.Views;

public partial class MyRentalsPage : ContentPage
{
    public MyRentalsPage()
    {
        InitializeComponent();

        // Force DI to resolve the ViewModel even when Shell creates the page
        BindingContext = ServiceHelper.GetService<MyRentalsViewModel>();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is MyRentalsViewModel vm)
            await vm.LoadRentalsCommand.ExecuteAsync(null);
    }
}
