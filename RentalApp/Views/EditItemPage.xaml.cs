using RentalApp.ViewModels;
using RentalApp.Services;

namespace RentalApp.Views;

public partial class EditItemPage : ContentPage
{
    // REQUIRED by MAUI for XAML + Shell route validation
    public EditItemPage()
    {
        InitializeComponent();
        BindingContext = App.Services.GetRequiredService<EditItemViewModel>();
    }

    // Used for DI
    public EditItemPage(EditItemViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
