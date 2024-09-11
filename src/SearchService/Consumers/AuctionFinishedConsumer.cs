using System;
using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Consumers;

public class AuctionFinishedConsumer : IConsumer<AuctionFinished>
{
    public async Task Consume(ConsumeContext<AuctionFinished> context)
    {
        Console.WriteLine($"Auction finished: {context.Message.AuctionId}.");

        var item =
            await DB.Find<Item>().OneAsync(context.Message.AuctionId)
            ?? throw new ArgumentException(
                $"Auction with id {context.Message.AuctionId} not found"
            );

        if (context.Message.ItemSold)
        {
            item.Winner = context.Message.Winner;
            item.SoldAmount = context.Message.Amount;
        }

        item.Status = "finished";

        await item.SaveAsync();
    }
}
