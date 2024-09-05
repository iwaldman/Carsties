using AuctionService.Dtos;
using AuctionService.Entities;
using AutoMapper;
using Contracts;

namespace AuctionService.RequestHelpers;

public class MappingProfiles : Profile
{
    /// <summary>
    /// Mapping profiles for AuctionService request helpers.
    /// </summary>
    public MappingProfiles()
    {
        // Maps properties from Auction to AuctionDto.
        // Includes members from the Item property of Auction.
        CreateMap<Auction, AuctionDto>()
            .IncludeMembers(x => x.Item);

        // Maps properties from Item to AuctionDto.
        CreateMap<Item, AuctionDto>();

        // Maps properties from CreateAuctionDto to Auction.
        // Maps the entire CreateAuctionDto object to the Item property of Auction.
        CreateMap<CreateAuctionDto, Auction>()
            .ForMember(d => d.Item, o => o.MapFrom(s => s));

        // Maps properties from CreateAuctionDto to Item.
        CreateMap<CreateAuctionDto, Item>();

        CreateMap<AuctionDto, AuctionCreated>();
        CreateMap<Auction, AuctionUpdated>().IncludeMembers(a => a.Item);
        CreateMap<Item, AuctionUpdated>();
    }
}
