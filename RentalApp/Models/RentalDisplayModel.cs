namespace RentalApp.Models;

public class RentalDisplayModel
{
    public LocalRental? Rental { get; set; }
    public string? ItemTitle { get; set; }
    public string? ItemImage { get; set; }
    public string? Category { get; set; }
    public decimal DailyRate { get; set; }
}
