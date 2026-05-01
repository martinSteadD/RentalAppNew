using RentalApp.ViewModels;

namespace RentalApp.Views;

public partial class AddReviewPage : ContentPage
{
    public AddReviewPage(AddReviewViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}