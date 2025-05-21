using TreniniApp.Models;

namespace TreniniApp.Services;

public interface IWebScrapingService
{
    Task<List<TrainRow>> GetTrainsAsync(string placeId);
}
