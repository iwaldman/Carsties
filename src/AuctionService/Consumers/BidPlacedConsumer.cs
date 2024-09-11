using System;
using AuctionService.Data;
using Contracts;
using MassTransit;

namespace AuctionService.Consumers;

public class BidPlacedConsumer(AuctionDbContext auctionDbContext) : IConsumer<BidPlaced>
{
    public async Task Consume(ConsumeContext<BidPlaced> context)
    {
        Console.WriteLine(
            $"Bid placed:  {context.Message.Amount} by {context.Message.Bidder} on auction {context.Message.AuctionId}."
        );

        var auction =
            await auctionDbContext.Auctions.FindAsync(context.Message.AuctionId)
            ?? throw new ArgumentException(
                $"Auction with id {context.Message.AuctionId} not found"
            );

        if (
            auction.CurrentHighBid is null
            || context.Message.BidStatus.Contains("Accepted")
                && context.Message.Amount > auction.CurrentHighBid
        )
        {
            auction.CurrentHighBid = context.Message.Amount;
            await auctionDbContext.SaveChangesAsync();
        }
    }
}
