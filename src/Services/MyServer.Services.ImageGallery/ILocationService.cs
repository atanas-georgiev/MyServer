namespace MyServer.Services.ImageGallery
{
    using System.Threading.Tasks;

    using MyServer.Data.Models;

    public interface ILocationService
    {
        Task<ImageGpsData> GetGpsData(string location);

        Task<ImageGpsData> GetGpsDataOriginal(double latitude, double longitude);

        Task<ImageGpsData> GetGpsDataNormalized(double latitude, double longitude);
    }
}