using System;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Services;

public class AuctionSvcHttpClient(HttpClient httpClient, IConfiguration configuration)
{
    public async Task<List<Item>> GetItemsForSearchDb()
    {
        var lastUpdated = await DB.Find<Item, string>()
            .Sort(x => x.Descending(x => x.UpdatedAt))
            .Project(x => x.UpdatedAt.ToString())
            .ExecuteFirstAsync();

        var auctionURL =
            configuration["AuctionServiceUrl"]
            ?? throw new ArgumentNullException("Cannot get auction address");

        var url = $"{auctionURL}/api/auctions";

        if (!string.IsNullOrEmpty(lastUpdated))
        {
            url += $"?date={lastUpdated}";
        }

        var items = await httpClient.GetFromJsonAsync<List<Item>>(url);

        return items ?? [];
    }
}
