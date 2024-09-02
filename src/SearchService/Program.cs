using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Data;
using SearchService.Models;
using SearchService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpClient<AuctionSvcHttpClient>();

var app = builder.Build();

app.MapControllers();

app.UseAuthorization();

try
{
    await DbInitializer.InitDbAsync(app);
}
catch (Exception e)
{
    Console.WriteLine(e);
}

app.Run();

public partial class Program { }
