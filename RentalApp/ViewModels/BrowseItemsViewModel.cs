using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RentalApp.Database.Models;
using RentalApp.Services;
using System.Collections.ObjectModel;

namespace RentalApp.ViewModels;

public partial class BrowseItemsViewModel : BaseViewModel
{
    private readonly IApiService _apiService;
    private readonly INavigationService _navigationService;

    public ObservableCollection<Item> Items { get; } = new();

    [ObservableProperty]
    private bool isRefreshing;

    public BrowseItemsViewModel(IApiService apiService, INavigationService navigationService)
    {
        Console.WriteLine("🔥 [BrowseItemsVM] CONSTRUCTOR HIT");

        _apiService = apiService;
        _navigationService = navigationService;

        Title = "Browse Items";

        Console.WriteLine("🔥 [BrowseItemsVM] Calling LoadItemsAsync from constructor");
        _ = LoadItemsAsync();
    }

    [RelayCommand]
    private async Task LoadItemsAsync()
    {
        Console.WriteLine("🔥 [BrowseItemsVM] LoadItemsAsync CALLED");

        if (IsBusy)
        {
            Console.WriteLine("⚠️ [BrowseItemsVM] LoadItemsAsync EXITED — IsBusy = true");
            return;
        }

        try
        {
            IsBusy = true;
            ClearError();

            Console.WriteLine("🌐 [BrowseItemsVM] Calling API: GetItemsAsync()");
            var items = await _apiService.GetItemsAsync();

            Console.WriteLine($"📦 [BrowseItemsVM] API returned {items?.Count()} items");

            Items.Clear();
            foreach (var item in items)
            {
                Console.WriteLine($"➕ [BrowseItemsVM] Adding item: {item?.Title} (ID: {item?.Id})");
                Items.Add(item);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ [BrowseItemsVM] ERROR: {ex}");
            SetError($"Failed to load items: {ex.Message}");
        }
        finally
        {
            Console.WriteLine("✅ [BrowseItemsVM] LoadItemsAsync FINISHED");
            IsBusy = false;
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        Console.WriteLine("🔄 [BrowseItemsVM] RefreshAsync CALLED");
        IsRefreshing = true;
        await LoadItemsAsync();
    }

    [RelayCommand]
    private async Task SelectItemAsync(Item item)
    {
        Console.WriteLine("🔥 [BrowseItemsVM] SelectItemAsync FIRED");

        if (item == null)
        {
            Console.WriteLine("⚠️ [BrowseItemsVM] SelectItemAsync EXITED — item was null");
            return;
        }

        Console.WriteLine($"➡️ [BrowseItemsVM] Navigating to itemdetails with item ID: {item.Id}");

        await _navigationService.NavigateToAsync("itemdetails", new Dictionary<string, object>
        {
            { "Item", item }
        });
    }
}
