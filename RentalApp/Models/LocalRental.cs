using SQLite;

namespace RentalApp.Models
{
    public class LocalRental
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        // API identifiers
        public int ApiRentalId { get; set; }
        public int ApiItemId { get; set; }

        // User info
        public int BorrowerId { get; set; }
        public int RequestedBy { get; set; }   // Local tracking: who created the request on this device

        // Rental details
        public string? Status { get; set; }
        public DateTime StartDate { get; set; }   // NEW
        public DateTime EndDate { get; set; }     // NEW

        // Metadata
        public DateTime LastSynced { get; set; }
    }
}
