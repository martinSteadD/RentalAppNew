namespace RentalApp.Database.Models
{
    public class NearbyItemsResponse
    {
        public List<Item> Items { get; set; } = new();
        public SearchLocation SearchLocation { get; set; }
        public int Radius { get; set; }
        public int TotalResults { get; set; }
    }

    public class SearchLocation
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
