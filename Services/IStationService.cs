using TreniniApp.Models;

namespace TreniniApp.Services;

public interface IStationService
{
    Task<string?> GetStationNameByIdAsync(string code);
    Task<IReadOnlyList<Station>> GetAllStationsAsync();
    Task<IReadOnlyList<Station>> SearchStationsAsync(string query);
}
