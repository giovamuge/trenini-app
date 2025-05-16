using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TreniniApp.Models;

namespace TreniniApp.Services;

public class StationService() : IStationService
{
    private readonly string _stationsFilePath = "stations.json";

    public async Task<string?> GetStationNameByIdAsync(string code)
    {
        var stations = await ReadStationsFromFileAsync();
        var station = stations.FirstOrDefault(s => s.Code == code);
        return station?.Name;
    }

    public async Task<IReadOnlyList<Station>> GetAllStationsAsync()
    {
        var stations = await ReadStationsFromFileAsync();
        return stations;
    }

    public async Task<IReadOnlyList<Station>> SearchStationsAsync(string query)
    {
        var stations = await ReadStationsFromFileAsync();
        if (string.IsNullOrWhiteSpace(query))
            return stations;

        query = query.ToLowerInvariant();
        return
        [
            .. stations.Where(s =>
                (
                    !string.IsNullOrEmpty(s.Name)
                    && s.Name.Contains(query, StringComparison.InvariantCultureIgnoreCase)
                )
                || (
                    !string.IsNullOrEmpty(s.Code)
                    && s.Code.Contains(query, StringComparison.InvariantCultureIgnoreCase)
                )
            )
        ];
    }

    private async Task<List<Station>> ReadStationsFromFileAsync()
    {
        if (!File.Exists(_stationsFilePath))
            return [];

        using var stream = File.OpenRead(_stationsFilePath);

        // Use source-generated context for AOT compatibility
        var stations = await JsonSerializer.DeserializeAsync(
            stream,
            StationJsonContext.Default.ListStation
        );
        return stations ?? [];
    }
}

// Add source generation context for AOT
[JsonSerializable(typeof(List<Station>))]
internal partial class StationJsonContext : JsonSerializerContext { }
