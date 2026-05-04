namespace RentalApp.Database.Models
{
    public class RentalRequest
    {
        public int itemId { get; set; }
        public string? startDate { get; set; }
        public string? endDate { get; set; }
    }
}
