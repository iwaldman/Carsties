using System;
using AuctionService.Data;
using Grpc.Core;

namespace AuctionService.Services;

public class GrpcAuctionService(AuctionDbContext auctionDbContext) : GrpcAuction.GrpcAuctionBase
{
    public override async Task<GrpcAuctionResponse> GetAuction(
        GetAuctionRequest request,
        ServerCallContext context
    )
    {
        Console.WriteLine("GetAuction called");

        var auction =
            await auctionDbContext.Auctions.FindAsync(Guid.Parse(request.Id))
            ?? throw new RpcException(new Status(StatusCode.NotFound, "Auction not found"));

        return new GrpcAuctionResponse
        {
            Auction = new GrpcAuctionModel
            {
                Id = auction.Id.ToString(),
                AuctionEnd = auction.AuctionEnd.ToString(),
                ReservePrice = auction.ReservePrice,
                Seller = auction.Seller
            }
        };
    }
}
