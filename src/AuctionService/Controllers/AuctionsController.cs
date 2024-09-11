using AuctionService.Data;
using AuctionService.Dtos;
using AuctionService.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers;

[ApiController]
[Route("api/auctions")]
public class AuctionsController(
    AuctionDbContext auctionDbContext,
    IMapper mapper,
    IPublishEndpoint publishEndpoint
) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions(string? date)
    {
        var query = auctionDbContext.Auctions.OrderBy(x => x.Item.Make).AsQueryable();

        if (!string.IsNullOrEmpty(date))
        {
            query = query.Where(
                x => x.UpdatedAt.Date.CompareTo(DateTime.Parse(date).ToUniversalTime()) > 0
            );
        }

        var auctions = await query
            .ProjectTo<AuctionDto>(mapper.ConfigurationProvider)
            .ToListAsync();

        return auctions;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id)
    {
        var auction = await auctionDbContext
            .Auctions.Include(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (auction is null)
        {
            return NotFound();
        }

        return mapper.Map<AuctionDto>(auction);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto createAuctionDto)
    {
        var auction = mapper.Map<Auction>(createAuctionDto);

        // TODO: add current user as the seller
        auction.Seller = User.Identity?.Name ?? "Unknown user";

        auctionDbContext.Auctions.Add(auction);

        var auctionDto = mapper.Map<AuctionDto>(auction);

        var auctionCreated = mapper.Map<AuctionCreated>(auctionDto);

        await publishEndpoint.Publish(auctionCreated);

        var result = await auctionDbContext.SaveChangesAsync() > 0;

        if (!result)
        {
            return BadRequest("Could not save changes to the DB.");
        }

        return CreatedAtAction(nameof(GetAuctionById), new { auction.Id }, auctionDto);
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDto updateAuctionDto)
    {
        var auction = await auctionDbContext
            .Auctions.Include(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (auction is null)
        {
            return NotFound();
        }

        if (auction.Seller != User.Identity?.Name)
            return Forbid();

        auction.Item.Make = updateAuctionDto.Make ?? auction.Item.Make;
        auction.Item.Model = updateAuctionDto.Model ?? auction.Item.Model;
        auction.Item.Color = updateAuctionDto.Color ?? auction.Item.Color;
        auction.Item.Mileage = updateAuctionDto.Mileage ?? auction.Item.Mileage;
        auction.Item.Year = updateAuctionDto.Year ?? auction.Item.Year;

        var auctionUpdated = mapper.Map<AuctionUpdated>(auction);

        await publishEndpoint.Publish(auctionUpdated);

        var result = await auctionDbContext.SaveChangesAsync() > 0;

        if (result)
        {
            return Ok();
        }

        return BadRequest("Problem saving changes");
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<ActionResult> DeleteAuction(Guid id)
    {
        var auction = await auctionDbContext.Auctions.FirstOrDefaultAsync(x => x.Id == id);

        if (auction is null)
        {
            return NotFound();
        }

        if (auction.Seller != User.Identity?.Name)
            return Forbid();

        auctionDbContext.Remove(auction);

        await publishEndpoint.Publish<AuctionDeleted>(
            new AuctionDeleted { Id = auction.Id.ToString() }
        );

        var result = await auctionDbContext.SaveChangesAsync() > 0;

        if (!result)
        {
            return BadRequest("Could not update DB.");
        }

        return Ok();
    }
}
