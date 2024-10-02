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
    IAuctionRepository auctionRepository,
    IMapper mapper,
    IPublishEndpoint publishEndpoint
) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions(string? date)
    {
        return await auctionRepository.GetAuctionsAsync(date);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id)
    {
        var auctionDto = await auctionRepository.GetAuctionByIdAsync(id);

        if (auctionDto is null)
        {
            return NotFound();
        }

        return auctionDto;
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto createAuctionDto)
    {
        var auction = mapper.Map<Auction>(createAuctionDto);

        // TODO: add current user as the seller
        auction.Seller = User.Identity?.Name ?? "Unknown user";

        auctionRepository.AddAuction(auction);

        var auctionDto = mapper.Map<AuctionDto>(auction);

        var auctionCreated = mapper.Map<AuctionCreated>(auctionDto);

        await publishEndpoint.Publish(auctionCreated);

        var result = await auctionRepository.SaveChangesAsync();

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
        var auction = await auctionRepository.GetAuctionEntityById(id);

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

        var result = await auctionRepository.SaveChangesAsync();

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
        var auction = await auctionRepository.GetAuctionEntityById(id);

        if (auction is null)
        {
            return NotFound();
        }

        if (auction.Seller != User.Identity?.Name)
            return Forbid();

        auctionRepository.RemoveAuction(auction);

        await publishEndpoint.Publish<AuctionDeleted>(
            new AuctionDeleted { Id = auction.Id.ToString() }
        );

        var result = await auctionRepository.SaveChangesAsync();

        if (!result)
        {
            return BadRequest("Could not update DB.");
        }

        return Ok();
    }
}
