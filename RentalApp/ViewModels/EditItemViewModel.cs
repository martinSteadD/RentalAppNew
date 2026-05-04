using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RentalApp.Database.Models;
using RentalApp.Repositories;
using System.Collections.ObjectModel;

namespace RentalApp.ViewModels;

[QueryProperty(nameof(Item), "Item")]
public partial class EditItemViewModel : BaseViewModel
{
    private readonly IItemRepository _items;

    [ObservableProperty]
    private Item? item;

    [ObservableProperty]
    private string title = string.Empty;

    [ObservableProperty]
    private string description = string.Empty;

    [ObservableProperty]
    private string dailyRate = string.Empty;

    [ObservableProperty]
    private ObservableCollection<Category> categories = new();

    [ObservableProperty]
    private Category? selectedCategory;

    public EditItemViewModel(IItemRepository items)
    {
        _items = items;
        Title = "Edit Item";
    }

    partial void OnItemChanged(Item? value)
    {
        if (value == null)
            return;

        Title = value.Title;
        Description = value.Description;
        DailyRate = value.DailyRate.ToString();

        _ = LoadCategoriesAsync();
    }

    private async Task LoadCategoriesAsync()
    {
        try
        {
            var list = await _items.GetCategoriesAsync();

            Categories = new ObservableCollection<Category>(list);

            if (Item != null)
                SelectedCategory = Categories.FirstOrDefault(c => c.Id == Item.CategoryId);
        }
        catch
        {
            await Shell.Current.DisplayAlert("Error", "Failed to load categories.", "OK");
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (Item == null)
            return;

        if (!decimal.TryParse(DailyRate, out var rate))
        {
            await Shell.Current.DisplayAlert("Error", "Daily rate must be a valid number.", "OK");
            return;
        }

        if (SelectedCategory == null)
        {
            await Shell.Current.DisplayAlert("Error", "Please select a category.", "OK");
            return;
        }

        try
        {
            IsBusy = true;
            ClearError();

            // Update editable fields
            Item.Title = Title;
            Item.Description = Description;
            Item.DailyRate = rate;
            Item.CategoryId = SelectedCategory.Id;
            Item.Category = SelectedCategory.Name;

            // Required fields are already present on Item and do not need reassignment

            await _items.UpdateAsync(Item);

            await Shell.Current.DisplayAlert("Success", "Item updated successfully!", "OK");
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            SetError($"Failed to update item: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
