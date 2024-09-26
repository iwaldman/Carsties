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
    public async Task<ActionResult<Bid>> PlaceBid(string auctionId, int amount)
    {
        var auction = await DB.Find<Auction>().OneAsync(auctionId);

        if (auction is null)
        {
            return BadRequest("Cannot accept bids on this auction at this time");
        }

        if (auction.Seller == User.Identity?.Name)
        {
            return BadRequest("You cannot bid on your own item");
        }

        if (User.Identity?.Name is null)
            return Unauthorized();

        var bid = new Bid()
        {
            Amount = amount,
            AuctionId = auctionId,
            Bidder = User.Identity.Name
        };

        if (auction.AuctionEnd < DateTime.UtcNow)
        {
            bid.BidStatus = BidStatus.Finished;
        }
        else
        {
            var highBid = await DB.Find<Bid>()
                .Match(a => a.AuctionId == auctionId)
                .Sort(b => b.Descending(x => x.Amount))
                .ExecuteFirstAsync();

            if (highBid != null && amount > highBid.Amount || highBid == null)
            {
                bid.BidStatus =
                    amount > auction.ReservePrice
                        ? BidStatus.Accepted
                        : BidStatus.AcceptedBelowReserve;
            }

            if (highBid != null && bid.Amount <= highBid.Amount)
            {
                bid.BidStatus = BidStatus.TooLow;
            }
        }

        await DB.SaveAsync(bid);

        return Ok();
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
