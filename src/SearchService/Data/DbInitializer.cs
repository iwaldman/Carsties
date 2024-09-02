using System.Text.Json;
using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Models;
using SearchService.Services;

namespace SearchService.Data;

public class DbInitializer
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions =
        new() { PropertyNameCaseInsensitive = true };

    public static async Task InitDbAsync(WebApplication app)
    {
        var connectionString = app.Configuration.GetConnectionString("MongoDbConnection");

        await DB.InitAsync("SearchDb", MongoClientSettings.FromConnectionString(connectionString))
            .ConfigureAwait(false);

        await DB.Index<Item>()
            .Key(s => s.Make, KeyType.Text)
            .Key(s => s.Model, KeyType.Text)
            .Key(s => s.Color, KeyType.Text)
            .CreateAsync()
            .ConfigureAwait(false);

        var count = await DB.CountAsync<Item>().ConfigureAwait(false);

        using var scope = app.Services.CreateScope();
        var httpClient = scope.ServiceProvider.GetRequiredService<AuctionSvcHttpClient>();
        var items = await httpClient.GetItemsForSearchDb().ConfigureAwait(false);
        Console.WriteLine($"{items.Count} items returned from the auction service");

        if (items.Count > 0)
        {
            Console.WriteLine("Seeding Search Service database...");

            await DB.SaveAsync(items);
        }

        // if (count == 0)
        // {
        //     Console.WriteLine("Seeding Search Service database...");

        //     var itemData = await File.ReadAllTextAsync("Data/auctions.json").ConfigureAwait(false);

        //     var items =
        //         JsonSerializer.Deserialize<List<Item>>(itemData, _jsonSerializerOptions) ?? [];

        //     await DB.SaveAsync(items).ConfigureAwait(false);
        // }
    }
}
