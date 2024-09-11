using System;
using AuctionService.Data;
using AuctionService.Entities;
using Contracts;
using MassTransit;

namespace AuctionService.Consumers;

public class AuctionFinishedConsumer(AuctionDbContext auctionDbContext) : IConsumer<AuctionFinished>
{
    public async Task Consume(ConsumeContext<AuctionFinished> context)
    {
        Console.WriteLine($"Auction finished: {context.Message.AuctionId}.");

        var auction = await auctionDbContext.Auctions.FindAsync(context.Message.AuctionId) ?? throw new ArgumentException($"Auction with id {context.Message.AuctionId} not found");
        
        if (context.Message.ItemSold)
        {
            auction.Winner = context.Message.Winner;
            auction.SoldAmount = context.Message.Amount;
        }

        auction.Status =
            auction.SoldAmount > auction.ReservePrice ? Status.Finished : Status.ReserveNotMet;

        await auctionDbContext.SaveChangesAsync();
    }
}
