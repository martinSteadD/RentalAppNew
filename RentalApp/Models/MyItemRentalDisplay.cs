namespace RentalApp.Models;

public class MyItemRentalDisplay
{
    public LocalItem Item { get; set; }
    public RentalApp.Database.Models.Rental Rental { get; set; }

    public string ItemTitle => Item?.Title;
    public string ItemImage => Item?.ImageUrl;
    public string Category => Item?.Category;
    public decimal DailyRate => Item?.DailyRate ?? 0;

    public string BorrowerName => Rental?.BorrowerName;
    public string DateRange => Rental?.DateRange;
    public string Status => Rental?.Status;
    public string TotalPriceFormatted => Rental?.TotalPriceFormatted;
}
