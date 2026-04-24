using RentalApp.ViewModels;

namespace RentalApp.Views;

public partial class MyRentalsPage : ContentPage
{
    public MyRentalsPage(MyRentalsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
