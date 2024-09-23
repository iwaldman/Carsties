using System;
using BiddingService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;

namespace BiddingService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BidsController : ControllerBase
{
    public BidsController() { }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<Bid>> Post(string auctionId, int amount)
    {
        var auction = await DB.Find<Auction>().OneAsync(auctionId);

        if (auction is null)
        {
            return NotFound();
        }

        if (auction.Seller == User.Identity?.Name)
        {
            return BadRequest("You cannot bid on your own auction.");
        }

        var bid = new Bid
        {
            AuctionId = auctionId,
            Amount = amount,
            Bidder = User.Identity?.Name!
        };

        if (auction.AuctionEnd < DateTime.UtcNow)
        {
            bid.BidStatus = BidStatus.Finished;
        }
        else
        {
            var highestBid = await DB.Find<Bid>()
                .Match(b => b.AuctionId == auctionId)
                .Sort(b => b.Descending(b => b.Amount))
                .Limit(1)
                .ExecuteFirstAsync();

            if (highestBid is not null && amount > highestBid.Amount || highestBid is null)
            {
                bid.BidStatus =
                    amount > auction.ReservePrice
                        ? BidStatus.Accepted
                        : BidStatus.AcceptedBelowReserve;
            }

            if (highestBid is not null && amount <= highestBid.Amount)
            {
                bid.BidStatus = BidStatus.TooLow;
            }
        }

        await DB.SaveAsync(bid);

        return Ok(bid);
    }

    [HttpGet("{auctionId}")]
    public async Task<ActionResult<IList<Bid>>> GetBidsForAuction(string auctionId)
    {
        var bids = await DB.Find<Bid>()
            .Match(b => b.AuctionId == auctionId)
            .Sort(b => b.Descending(a => a.BidTime))
            .ExecuteAsync();

        return Ok(bids);
    }
}
