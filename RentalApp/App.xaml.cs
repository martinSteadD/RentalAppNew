using RentalApp.ViewModels;

namespace RentalApp;

public partial class App : Application
{
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

        // Navigate to login AFTER shell is loaded
       MainThread.BeginInvokeOnMainThread(async () =>
        {
            await Task.Delay(100); // allow Shell to attach
            await Shell.Current.GoToAsync("login");
        });


        return window;
    }
}
