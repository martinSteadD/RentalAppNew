using Microsoft.Extensions.Logging;
using RentalApp.ViewModels;
using RentalApp.Database.Data;
using RentalApp.Views;
using System.Diagnostics;
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
        builder.Services.AddSingleton<INavigationService, NavigationService>();

        // Coursework API services
        builder.Services.AddSingleton<IApiService, ApiService>();
        builder.Services.AddSingleton<IRentalService, RentalService>();
        builder.Services.AddSingleton<ILocationService, LocationService>();

        // Database
        builder.Services.AddSingleton<DatabaseService>();

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
        builder.Services.AddTransient<TempViewModel>();
        builder.Services.AddTransient<TempPage>();
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
        builder.Services.AddTransient<UserListViewModel>();
        builder.Services.AddTransient<UserListPage>();
        builder.Services.AddTransient<AboutViewModel>();
        builder.Services.AddTransient<AboutPage>();
        builder.Services.AddTransient<MyItemsViewModel>();
        builder.Services.AddTransient<MyItemsPage>();
        builder.Services.AddTransient<RentalRequestsViewModel>();
        builder.Services.AddTransient<RentalRequestsPage>();
        builder.Services.AddTransient<RentalDetailsViewModel>();
        builder.Services.AddTransient<RentalDetailsPage>();

#if DEBUG
        builder.Logging.AddDebug();   // ✔ MUST be BEFORE Build()
#endif

        var app = builder.Build();

        // Initialize database safely without blocking UI
        Task.Run(async () =>
        {
            var db = app.Services.GetRequiredService<DatabaseService>();
            await db.InitializeAsync();
        });

        return app;
    }
}
