using RentalApp.ViewModels;

namespace RentalApp.Views
{
    public partial class MyItemsPage : ContentPage
    {
        public MyItemsPage(MyItemsViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is MyItemsViewModel vm)
                await vm.LoadItemsAsync();
        }
    }
}