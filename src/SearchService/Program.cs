using System.Net;
using MassTransit;
using Polly;
using Polly.Extensions.Http;
using SearchService.Consumers;
using SearchService.Data;
using SearchService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddHttpClient<AuctionSvcHttpClient>().AddPolicyHandler(GetRetryPolicy());

builder.Services.AddMassTransit(x =>
{
    x.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();

    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("search", false));

    x.UsingRabbitMq(
        (context, cfg) =>
        {
            cfg.Host(
                builder.Configuration["RabbitMQ:Host"],
                "/",
                h =>
                {
                    h.Username(builder.Configuration.GetValue("RabbitMQ:Username", "guest")!);
                    h.Password(builder.Configuration.GetValue("RabbitMQ:Password", "guest")!);
                }
            );

            cfg.ReceiveEndpoint(
                "search-auction-created",
                e =>
                {
                    e.UseMessageRetry(r => r.Interval(5, 5));

                    e.ConfigureConsumer<AuctionCreatedConsumer>(context);
                }
            );

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
