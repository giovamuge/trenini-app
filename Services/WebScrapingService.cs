using HtmlAgilityPack;
using TreniniApp.Models;

namespace TreniniApp.Services;

public class WebScrapingService(HttpClient httpClient) : IWebScrapingService
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<List<TrainRow>> GetTrainsAsync(string placeId)
    {
        try
        {
            var html = await _httpClient.GetStringAsync(
                $"https://iechub.rfi.it/ArriviPartenze/ArrivalsDepartures/Monitor?Arrivals=False&Search=&PlaceId={placeId}"
            );
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var rows = new List<TrainRow>();

            // Example XPath to find the table rows (adjust as needed based on actual DOM)
            var tableRows = doc.DocumentNode.SelectNodes("//table//tr");
            if (tableRows == null)
                return rows;

            var index = 0;
            var tableRowsWithoutHeader = tableRows.Skip(1);
            foreach (var row in tableRowsWithoutHeader) // skip header if needed
            {
                var cells = row.SelectNodes("td");
                if (cells == null || cells.Count < 5)
                    continue;

                var time = cells[4].InnerText.Trim();
                var destination = cells[3].InnerText.Trim();
                var train = cells[2].InnerText.Trim();
                var delay = cells[5].InnerText.Trim();
                var track = cells[6].InnerText.Trim();

                // get the category and vector images
                var category = cells[1]
                    .SelectNodes("img")
                    ?.FirstOrDefault()
                    ?.GetAttributeValue("src", string.Empty);

                // get the vector image
                var vect = cells[0]
                    .SelectNodes("img")
                    ?.FirstOrDefault()
                    ?.GetAttributeValue("src", string.Empty);

                var position =
                    index == 0
                        ? Position.First
                        : index == tableRowsWithoutHeader.Count() - 1
                            ? Position.Last
                            : Position.Middle;

                rows.Add(
                    new TrainRow(
                        time,
                        train,
                        destination,
                        track,
                        delay,
                        category,
                        vect,
                        index,
                        position
                    )
                );

                index++;
            }

            // Remove empty rows
            rows =
            [
                .. rows.Where(r =>
                    !string.IsNullOrWhiteSpace(r.Time)
                    && !string.IsNullOrWhiteSpace(r.Destination)
                    && !string.IsNullOrWhiteSpace(r.Train)
                )
            ];

            return rows;
        }
        catch (HttpRequestException e)
        {
            // Handle HTTP request exceptions
            Console.WriteLine($"HTTP Request Exception: {e.Message}");
            return [];
        }
        catch (Exception e)
        {
            // Handle other exceptions
            Console.WriteLine($"General Exception: {e.Message}");
            return [];
        }
    }
}
