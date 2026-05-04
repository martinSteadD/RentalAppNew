using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RentalApp.Database.Models;
using RentalApp.Repositories;
using System.Collections.ObjectModel;

namespace RentalApp.ViewModels;

public partial class BrowseItemsViewModel : BaseViewModel
{
    private readonly IItemRepository _items;

    public ObservableCollection<Item> Items { get; } = new();

    [ObservableProperty]
    private bool isRefreshing;

    public BrowseItemsViewModel(IItemRepository items)
    {
        _items = items;

        Title = "Browse Items";

        _ = LoadItemsAsync();
    }

    [RelayCommand]
    private async Task LoadItemsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            ClearError();

            var list = await _items.GetAllAsync();

            Items.Clear();
            foreach (var item in list)
                Items.Add(item);
        }
        catch (Exception ex)
        {
            SetError($"Failed to load items: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        IsRefreshing = true;
        await LoadItemsAsync();
    }

    [RelayCommand]
    private async Task SelectItemAsync(Item item)
    {
        if (item == null)
            return;

        await Shell.Current.GoToAsync("itemdetails", true, new Dictionary<string, object>
        {
            { "Item", item }
        });
    }
}
