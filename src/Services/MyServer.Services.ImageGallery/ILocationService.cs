namespace MyServer.Services.ImageGallery
{
    using System.Threading.Tasks;

    using MyServer.Data.Models;

    public interface ILocationService
    {
        Task<ImageGpsData> GetGpsData(string location);

        Task<ImageGpsData> GetGpsData(double latitude, double longitude);
    }
}