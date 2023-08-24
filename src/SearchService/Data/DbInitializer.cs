using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Models;
using SearchService.Services;

namespace SearchService.Data;

public class DbInitializer
{
    public static async Task InitDbAsync(WebApplication app)
    {
        await DB.InitAsync(
            "SearchServiceDB",
            MongoClientSettings.FromConnectionString(
                app.Configuration.GetConnectionString("MongoDBConnection")
            )
        );

        await DB.Index<Item>()
            .Key(x => x.Make, KeyType.Text)
            .Key(x => x.Model, KeyType.Text)
            .Key(x => x.Color, KeyType.Text)
            .CreateAsync();

        var count = await DB.CountAsync<Item>();

        if (count == 0)
        {
            var scope = app.Services.CreateScope();

            var auctionSvcHttpClient =
                scope.ServiceProvider.GetRequiredService<AuctionSvcHttpClient>();

            var items = await auctionSvcHttpClient.GetItemsForSearchDb();

            Console.WriteLine($"Seeding database with {items.Count} items...");

            await DB.SaveAsync(items);
        }

        // if (count == 0)
        // {
        //     Console.WriteLine("Seeding database...");

        //     var itemData = await File.ReadAllTextAsync("Data/auctions.json");
        //     var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        //     var items = JsonSerializer.Deserialize<Item[]>(itemData, options);
        //     await DB.SaveAsync(items);
        // }
    }
}
