var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.UseAuthorization();

app.Run();

public partial class Program { }
