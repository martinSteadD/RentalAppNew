using RentalApp.ViewModels;

namespace RentalApp.Views;

public partial class ItemDetailsPage : ContentPage
{
    public ItemDetailsPage(ItemDetailsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
