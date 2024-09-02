using System.Net;
using MassTransit;
using Polly;
using Polly.Extensions.Http;
using SearchService.Data;
using SearchService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddHttpClient<AuctionSvcHttpClient>().AddPolicyHandler(GetRetryPolicy());

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq(
        (context, cfg) =>
        {
            cfg.ConfigureEndpoints(context);
        }
    );
});

var app = builder.Build();

app.MapControllers();

app.UseAuthorization();

app.Lifetime.ApplicationStarted.Register(async () =>
{
    try
    {
        await DbInitializer.InitDbAsync(app);
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
    }
});

app.Run();

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy() =>
    HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
        .WaitAndRetryForeverAsync(_ => TimeSpan.FromSeconds(3));

public partial class Program { }
