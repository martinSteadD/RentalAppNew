namespace RentalApp.Database.Models
{
    public class Rental
    {
        public int Id { get; set; }

        public int ItemId { get; set; }
        public string? ItemTitle { get; set; }
        public string? ItemDescription { get; set; }

        public int BorrowerId { get; set; }
        public string? BorrowerName { get; set; }
        public decimal? BorrowerRating { get; set; }

        public int OwnerId { get; set; }
        public string? OwnerName { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string? Status { get; set; }
        public decimal TotalPrice { get; set; }

        public DateTime RequestedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }

        public string DateRange => $"{StartDate:dd MMM} - {EndDate:dd MMM}";
        public string TotalPriceFormatted => $"£{TotalPrice:F2}";
    }
}
