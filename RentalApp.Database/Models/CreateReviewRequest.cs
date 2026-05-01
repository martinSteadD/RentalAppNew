using System.Text.Json.Serialization;
namespace RentalApp.Database.Models;

public class CreateReviewRequest
{
    public int RentalId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
}
