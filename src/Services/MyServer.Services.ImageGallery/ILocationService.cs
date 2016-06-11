namespace MyServer.Services.ImageGallery
{
    using MyServer.Data.Models.ImageGallery;

    public interface ILocationService
    {
        ImageGpsData GetGpsData(string location);

        ImageGpsData GetGpsData(double latitude, double longitude);
    }
}