using RentalApp.ViewModels;

namespace RentalApp.Views;

public partial class BrowseItemsPage : ContentPage
{
    public BrowseItemsPage(BrowseItemsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
