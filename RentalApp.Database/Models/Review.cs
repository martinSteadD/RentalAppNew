namespace RentalApp.Database.Models
{
    public class Review
    {
        public int Id { get; set; }

        public int ItemId { get; set; }
        public int ReviewerId { get; set; }

        public string? ReviewerName { get; set; }
        public double Rating { get; set; }
        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
