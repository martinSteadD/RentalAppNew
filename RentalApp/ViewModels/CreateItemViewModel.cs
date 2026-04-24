using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RentalApp.Database.Models;
using RentalApp.Services;
using System.Collections.ObjectModel;

namespace RentalApp.ViewModels;

public partial class CreateItemViewModel : BaseViewModel
{
    private readonly IItemService _itemService;
    private readonly ICategoryService _categoryService;
    private readonly INavigationService _navigationService;
    private readonly IAuthenticationService _authService;

    [ObservableProperty]
    private string titleText = string.Empty;

    [ObservableProperty]
    private string description = string.Empty;

    [ObservableProperty]
    private string dailyRate = string.Empty;

    public ObservableCollection<Category> Categories { get; } = new();

    [ObservableProperty]
    private Category? selectedCategory;

    public CreateItemViewModel(
        IItemService itemService,
        ICategoryService categoryService,
        IAuthenticationService authService,
        INavigationService navigationService)
    {
        _itemService = itemService;
        _categoryService = categoryService;
        _authService = authService;
        _navigationService = navigationService;

        Title = "Create Item";

        LoadCategories();
    }

    private async void LoadCategories()
    {
        try
        {
            var list = await _categoryService.GetCategoriesAsync();

            Categories.Clear();
            foreach (var c in list)
                Categories.Add(c);
        }
        catch
        {
            await Shell.Current.DisplayAlertAsync("Error", "Failed to load categories.", "OK");
        }
    }

    [RelayCommand]
    private async Task CreateItemAsync()
    {
        if (IsBusy)
            return;

        if (string.IsNullOrWhiteSpace(TitleText) ||
            string.IsNullOrWhiteSpace(Description) ||
            string.IsNullOrWhiteSpace(DailyRate) ||
            SelectedCategory == null)
        {
            SetError("Please fill in all required fields.");
            return;
        }

        try
        {
            IsBusy = true;
            ClearError();

            var item = new Item
            {
                Title = TitleText,
                Description = Description,
                DailyRate = decimal.Parse(DailyRate),
                CategoryId = SelectedCategory.Id,
                Category = SelectedCategory.Name,
                OwnerId = _authService.CurrentUser!.Id,
                OwnerName = $"{_authService.CurrentUser.FirstName} {_authService.CurrentUser.LastName}",
                IsAvailable = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _itemService.CreateItemAsync(item);

            if (result == null)
            {
                SetError("Failed to create item.");
                return;
            }

            await Shell.Current.DisplayAlertAsync("Success", "Item created successfully!", "OK");
            await _navigationService.NavigateBackAsync();
        }
        catch (Exception ex)
        {
            SetError($"Failed to create item: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
