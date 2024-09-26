using System;
using BiddingService.Models;
using Contracts;
using MassTransit;
using MongoDB.Entities;

namespace BiddingService.Services;

public class CheckAuctionFinished(
    ILogger<CheckAuctionFinished> logger,
    IServiceProvider serviceProvider
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("CheckAuctionFinished is starting.");

        stoppingToken.Register(() => logger.LogInformation("CheckAuctionFinished is stopping."));

        while (!stoppingToken.IsCancellationRequested)
        {
            await CheckAuctions(stoppingToken);

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }

    private async Task CheckAuctions(CancellationToken stoppingToken)
    {
        logger.LogInformation("Checking auctions");

        var finishedAuctions = await DB.Find<Auction>()
            .Match(a => a.AuctionEnd <= DateTime.UtcNow)
            .Match(a => !a.Finished)
            .ExecuteAsync();

        logger.LogInformation("Found {Count} finished auctions", finishedAuctions.Count);

        if (finishedAuctions.Count == 0)
        {
            return;
        }

        using var scope = serviceProvider.CreateScope();
        var endpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

        foreach (var auction in finishedAuctions)
        {
            auction.Finished = true;
            await auction.SaveAsync(null, stoppingToken);

            var winningBid = await DB.Find<Bid>()
                .Match(b => b.AuctionId == auction.ID)
                .Match(b => b.BidStatus == BidStatus.Accepted)
                .Sort(b => b.Descending(b => b.Amount))
                .ExecuteFirstAsync(stoppingToken);

            await endpoint.Publish(
                new AuctionFinished
                {
                    ItemSold = winningBid != null,
                    AuctionId = auction.ID,
                    Winner = winningBid?.Bidder,
                    Amount = winningBid?.Amount,
                    Seller = auction.Seller
                },
                stoppingToken
            );
        }
    }
}
