using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Data;
using SearchService.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

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
