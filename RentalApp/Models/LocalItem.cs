using SQLite;

namespace RentalApp.Models
{
    public class LocalItem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        // API identifiers
        public int ApiItemId { get; set; }
        public int CreatedBy { get; set; }   // TRUE owner (OwnerId from API)

        // Core item details
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }

        // Category info
        public int CategoryId { get; set; }
        public string? Category { get; set; }

        // Rental info
        public decimal DailyRate { get; set; }
        public bool IsAvailable { get; set; }

        // Owner info
        public string? OwnerName { get; set; }
        public double OwnerRating { get; set; }

        // Ratings
        public double AverageRating { get; set; }

        // Metadata
        public DateTime CreatedAt { get; set; }
        public DateTime LastSynced { get; set; }

        // Location (optional but supported by API)
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
