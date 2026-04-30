namespace RentalApp.Database.Models
{
    public class OutgoingRentalsResponse
    {
        public List<Rental> Rentals { get; set; } = new();
        public int TotalRentals { get; set; }
    }
}
