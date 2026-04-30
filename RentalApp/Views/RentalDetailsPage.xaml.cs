using RentalApp.ViewModels;

namespace RentalApp.Views;

public partial class RentalDetailsPage : ContentPage
{
    public RentalDetailsPage(RentalDetailsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

}
