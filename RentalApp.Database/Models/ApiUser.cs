namespace RentalApp.Database.Models
{
    public class ApiUser
    {
        public int Id { get; set; }
        public string Email { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public double AverageRating { get; set; }
        public int ItemsListed { get; set; }
        public int RentalsCompleted { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
