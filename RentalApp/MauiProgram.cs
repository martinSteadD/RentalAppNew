using Microsoft.Extensions.Logging;
using RentalApp.ViewModels;
using RentalApp.Repositories;
using RentalApp.Views;
using RentalApp.Services;

namespace RentalApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Core services
        builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();

        // API services
        builder.Services.AddSingleton<IApiService, ApiService>();
        builder.Services.AddSingleton<ILocationService, LocationService>();
        builder.Services.AddTransient<IReviewService, ReviewService>();

        // Repositories
        builder.Services.AddSingleton<IItemRepository, ItemRepository>();
        builder.Services.AddSingleton<IRentalRepository, RentalRepository>();
        // Remove if unused:
        // builder.Services.AddSingleton<IReviewRepository, ReviewRepository>();

        // Shell + App
        builder.Services.AddSingleton<AppShellViewModel>();
        builder.Services.AddSingleton<AppShell>();
        builder.Services.AddSingleton<App>();

        // Pages + ViewModels
        builder.Services.AddTransient<MainViewModel>();
        builder.Services.AddTransient<MainPage>();

        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<LoginPage>();

        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<RegisterPage>();

        builder.Services.AddTransient<ItemDetailsViewModel>();
        builder.Services.AddTransient<ItemDetailsPage>();

        builder.Services.AddTransient<BrowseItemsViewModel>();
        builder.Services.AddTransient<BrowseItemsPage>();

        builder.Services.AddTransient<CreateItemViewModel>();
        builder.Services.AddTransient<CreateItemPage>();

        builder.Services.AddTransient<MyRentalsViewModel>();
        builder.Services.AddTransient<MyRentalsPage>();

        builder.Services.AddTransient<ProfileViewModel>();
        builder.Services.AddTransient<ProfilePage>();

        builder.Services.AddTransient<MyItemsViewModel>();
        builder.Services.AddTransient<MyItemsPage>();

        builder.Services.AddTransient<RentalRequestsViewModel>();
        builder.Services.AddTransient<RentalRequestsPage>();

        builder.Services.AddTransient<RentalDetailsViewModel>();
        builder.Services.AddTransient<RentalDetailsPage>();

        builder.Services.AddTransient<NearbyItemsViewModel>();
        builder.Services.AddTransient<NearbyItemsPage>();

        builder.Services.AddTransient<EditItemViewModel>();
        builder.Services.AddTransient<EditItemPage>();

        //builder.Services.AddTransient<RequestRentalViewModel>();
        //builder.Services.AddTransient<RequestRentalPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
