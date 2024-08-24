using System;
using AuctionService.Data;
using AuctionService.Dtos;
using AuctionService.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers;

[ApiController]
[Route("api/auctions")]
public class AuctionsController(AuctionDbContext auctionDbContext, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions()
    {
        var auctions = await auctionDbContext
            .Auctions.Include(x => x.Item)
            .OrderBy(x => x.Item.Make)
            .ToListAsync();

        return mapper.Map<List<AuctionDto>>(auctions);
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
    public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto createAuctionDto)
    {
        var auction = mapper.Map<Auction>(createAuctionDto);

        // TODO: add current user as the seller
        auction.Seller = "test";

        auctionDbContext.Auctions.Add(auction);
        var result = await auctionDbContext.SaveChangesAsync() > 0;

        if (!result)
        {
            return BadRequest("Could not save changes to the DB.");
        }

        var auctionDto = mapper.Map<AuctionDto>(auction);

        return CreatedAtAction(nameof(GetAuctionById), new { auction.Id }, auctionDto);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDto updateAuctionDto)
    {
        var auction = await auctionDbContext
            .Auctions.Include(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == id);
            
        if (auction is null)
        {
            return NotFound();
        }

        if (auction.Seller != "test")
        {
            return Forbid();
        }

        auction.Item.Make = updateAuctionDto.Make ?? auction.Item.Make;
        auction.Item.Model = updateAuctionDto.Model ?? auction.Item.Model;
        auction.Item.Color = updateAuctionDto.Color ?? auction.Item.Color;
        auction.Item.Mileage = updateAuctionDto.Mileage ?? auction.Item.Mileage;
        auction.Item.Year = updateAuctionDto.Year ?? auction.Item.Year;

        var result = await auctionDbContext.SaveChangesAsync() > 0;

        if (result)
        {
            return Ok();
        }

        return BadRequest("Problem saving changes");
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteAuction(Guid id)
    {
        var auction = await auctionDbContext.Auctions.FirstOrDefaultAsync(x => x.Id == id);

        if (auction is null)
        {
            return NotFound();
        }

        auctionDbContext.Remove(auction);

        var result = await auctionDbContext.SaveChangesAsync() > 0;

        if (!result)
        {
            return BadRequest("Could not update DB.");
        }

        return Ok();
    }
}