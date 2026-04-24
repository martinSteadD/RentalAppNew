using Microsoft.Maui.Devices.Sensors;

namespace RentalApp.Services
{
    public class LocationService : ILocationService
    {
        public async Task<(double Latitude, double Longitude)> GetCurrentLocationAsync()
        {
            var request = new GeolocationRequest(GeolocationAccuracy.Medium);
            var location = await Geolocation.GetLocationAsync(request);

            if (location == null)
                throw new Exception("Unable to get device location.");

            return (location.Latitude, location.Longitude);
        }
    }
}
