using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

await DB.InitAsync(
    "SearchDb",
    MongoClientSettings.FromConnectionString(
        builder.Configuration.GetConnectionString("MongoDbConnection")
    )
);

await DB.Index<Item>()
    .Key(s => s.Make, KeyType.Text)
    .Key(s => s.Model, KeyType.Text)
    .Key(s => s.Color, KeyType.Text)
    .CreateAsync();

app.UseAuthorization();

app.Run();

public partial class Program { }
