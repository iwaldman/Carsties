using System;
using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Consumers;

public class BidPlacedConsumer : IConsumer<BidPlaced>
{
    public async Task Consume(ConsumeContext<BidPlaced> context)
    {
        Console.WriteLine(
            $"Bid placed:  {context.Message.Amount} by {context.Message.Bidder} on auction {context.Message.AuctionId}."
        );

        var auction =
            await DB.Find<Item>().OneAsync(context.Message.AuctionId)
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
            await auction.SaveAsync();
        }
    }
}
