using RentalApp.ViewModels;

namespace RentalApp.Views;

public partial class BrowseItemsPage : ContentPage
{
    public BrowseItemsPage(BrowseItemsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        Console.WriteLine("📱 [BrowseItemsPage] OnAppearing FIRED");
    }

}
