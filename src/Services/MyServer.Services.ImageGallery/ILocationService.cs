namespace MyServer.Services.ImageGallery
{
    using MyServer.Data.Models;
    using System.Threading.Tasks;

    public interface ILocationService
    {
        Task<ImageGpsData> GetGpsData(string location);

        Task<ImageGpsData> GetGpsData(double latitude, double longitude);
    }
}