using System;
using AuctionService.Data;
using AuctionService.Dtos;
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
}
