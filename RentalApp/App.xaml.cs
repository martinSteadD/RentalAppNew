using RentalApp.ViewModels;
using RentalApp.Services;

namespace RentalApp;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; }
    private readonly IServiceProvider _serviceProvider;

    public App(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var shell = _serviceProvider.GetRequiredService<AppShell>();
        var window = new Window(shell);

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await Task.Delay(100); // allow Shell to attach

            // ⭐ JUST navigate to login
            await Shell.Current.GoToAsync("login");
        });

        return window;
    }
}
