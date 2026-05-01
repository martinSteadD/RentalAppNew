using RentalApp.ViewModels;

namespace RentalApp.Views;

public partial class ItemDetailsPage : ContentPage
{
    public ItemDetailsPage(ItemDetailsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
{
    base.OnNavigatedTo(args);

    if (BindingContext is ItemDetailsViewModel vm)
        await vm.LoadAsync();
}

}
