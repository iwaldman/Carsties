using System;
using Contracts;
using MassTransit;

namespace AuctionService.Consumers;

public class AuctionCreatedFaultConsumer : IConsumer<Fault<AuctionCreated>>
{
    public async Task Consume(ConsumeContext<Fault<AuctionCreated>> context)
    {
        Console.WriteLine($"AuctionCreated fault: {context.Message.Message}");

        var exception = context.Message.Exceptions.FirstOrDefault();

        if (exception?.ExceptionType == typeof(ArgumentException).Name)
        {
            context.Message.Message.Model = "FooBar";
            await context.Publish<AuctionCreated>(context.Message.Message);
        }
        else
        {
            Console.WriteLine($"Not handling exception: {exception?.ExceptionType}");
        }
    }
}
