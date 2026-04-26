using RentalApp.ViewModels;
using RentalApp.Views;

namespace RentalApp;

public partial class AppShell : Shell
{
    public AppShell()
    {   
        InitializeComponent();

        // Register navigation routes
        Routing.RegisterRoute("browse", typeof(BrowseItemsPage));
        Routing.RegisterRoute("itemdetails", typeof(ItemDetailsPage));
        Routing.RegisterRoute("createitem", typeof(CreateItemPage));
        Routing.RegisterRoute("myrentals", typeof(MyRentalsPage));
        Routing.RegisterRoute("profile", typeof(ProfilePage));
        Routing.RegisterRoute("login", typeof(LoginPage));
        Routing.RegisterRoute("userlist", typeof(UserListPage));
        Routing.RegisterRoute("about", typeof(AboutPage));
        Routing.RegisterRoute("register",typeof(RegisterPage));
        Routing.RegisterRoute("main", typeof(MainPage));
        Routing.RegisterRoute("myItems", typeof(MyItemsPage));
        Routing.RegisterRoute("rentalrequests", typeof(RentalRequestsPage));


    }
}
