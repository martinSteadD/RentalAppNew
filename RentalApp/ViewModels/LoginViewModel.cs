using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RentalApp.Services;

namespace RentalApp.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly IAuthenticationService _authService;

        [ObservableProperty]
        private string email = string.Empty;

        [ObservableProperty]
        private string password = string.Empty;

        [ObservableProperty]
        private bool rememberMe;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private bool hasError;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        public LoginViewModel(IAuthenticationService authService)
        {
            _authService = authService;
            HasError = false;
        }

        [RelayCommand]
        private async Task LoginAsync()
        {
            if (IsBusy)
                return;

            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Please enter both email and password.";
                HasError = true;
                return;
            }

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;
                HasError = false;

                var token = await _authService.LoginAsync(Email, Password);

                if (!string.IsNullOrWhiteSpace(token))
                {
                    await Shell.Current.GoToAsync("main");
                }
                else
                {
                    ErrorMessage = "Invalid email or password.";
                    HasError = true;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Login failed: {ex.Message}";
                HasError = true;
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task NavigateToRegisterAsync()
        {
            await Shell.Current.GoToAsync("register");
        }
    }
}
