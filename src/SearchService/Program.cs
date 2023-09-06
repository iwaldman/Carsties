using System.Net;
using MassTransit;
using Polly;
using Polly.Extensions.Http;
using SearchService.Consumers;
using SearchService.Data;
using SearchService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddHttpClient<AuctionSvcHttpClient>().AddPolicyHandler(GetRetryPolicy());

builder.Services.AddMassTransit(options =>
{
    // Add the consumer to the container
    options.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();

    options.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("search", false));

    // Set up the RabbitMq connection
    options.UsingRabbitMq(
        (context, cfg) =>
        {
            // Configure RabbitMq
            cfg.Host(
                builder.Configuration["RabbitMq:Host"],
                "/",
                host =>
                {
                    host.Username(builder.Configuration.GetValue("RabbitMq:User", "guest"));
                    host.Password(builder.Configuration.GetValue("RabbitMq:Password", "guest"));
                }
            );

            cfg.ReceiveEndpoint(
                "search-auction-created",
                e =>
                {
                    e.UseMessageRetry(r => r.Interval(5, TimeSpan.FromSeconds(5)));
                    e.ConfigureConsumer<AuctionCreatedConsumer>(context);
                }
            );
            cfg.ConfigureEndpoints(context);
        }
    );
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Lifetime.ApplicationStarted.Register(async () =>
{
    try
    {
        await DbInitializer.InitDbAsync(app);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Failed to initialize database: {ex.Message}");
        return;
    }

    Console.WriteLine("Application started.");
});

app.Run();

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy() =>
    HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
        .WaitAndRetryForeverAsync(_ => TimeSpan.FromSeconds(3));
