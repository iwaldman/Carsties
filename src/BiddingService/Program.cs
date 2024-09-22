using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MongoDB.Driver;
using MongoDB.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddMassTransit(x =>
{
    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("bids", false));

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

            cfg.ConfigureEndpoints(context);
        }
    );
});

builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["IdentityServiceUrl"];
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters.ValidateAudience = false;
        options.TokenValidationParameters.NameClaimType = "username";
    });

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

var connectionString = app.Configuration.GetConnectionString("BidDbConnection");

await DB.InitAsync("BidDB", MongoClientSettings.FromConnectionString(connectionString))
    .ConfigureAwait(false);

app.Run();