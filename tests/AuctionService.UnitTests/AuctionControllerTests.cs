using AuctionService.Controllers;
using AuctionService.Data;
using AuctionService.Dtos;
using AuctionService.Entities;
using AuctionService.RequestHelpers;
using AutoFixture;
using AutoMapper;
using MassTransit;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AuctionService.UnitTests;

// Method_Scenario_ExpectedBehaviour

public class AuctionControllerTests
{
    private readonly Mock<IAuctionRepository> _auctionRepositoryMock;
    private readonly IMapper _mapperMock;
    private readonly AuctionsController _auctionController;
    private readonly Mock<IPublishEndpoint> _publishEndpointMock;
    private readonly Fixture _fixture;

    public AuctionControllerTests()
    {
        _auctionRepositoryMock = new Mock<IAuctionRepository>();

        _publishEndpointMock = new Mock<IPublishEndpoint>();

        _fixture = new Fixture();

        var mockWrapper = new MapperConfiguration(cfg =>
        {
            cfg.AddMaps(typeof(MappingProfiles).Assembly);
        }).CreateMapper().ConfigurationProvider;

        _mapperMock = new Mapper(mockWrapper);

        _auctionController = new AuctionsController(
            _auctionRepositoryMock.Object,
            _mapperMock,
            _publishEndpointMock.Object
        );
    }

    [Fact]
    public async Task GetAllAuctions_WithNoParams_Returns10Auctions()
    {
        // arrange
        var auctions = _fixture.CreateMany<AuctionDto>(10).ToList();
        _auctionRepositoryMock.Setup(repo => repo.GetAuctionsAsync(null)).ReturnsAsync(auctions);

        // act
        var result = await _auctionController.GetAllAuctions(null);

        // assert
        Assert.Equal(10, result.Value?.Count);
        Assert.IsType<ActionResult<List<AuctionDto>>>(result);
    }

    [Fact]
    public async Task GetAuctionById_WithValidGuid_ReturnsAuction()
    {
        // arrange
        var auction = _fixture.Create<AuctionDto>();

        _auctionRepositoryMock
            .Setup(repo => repo.GetAuctionByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(auction);

        // act
        var result = await _auctionController.GetAuctionById(auction.Id);

        // assert
        Assert.Equal(auction.Id, result.Value?.Id);
        Assert.IsType<ActionResult<AuctionDto>>(result);
    }

    [Fact]
    public async Task GetAuctionById_WithInValidGuid_ReturnsNotFound()
    {
        // arrange
        _auctionRepositoryMock
            .Setup(repo => repo.GetAuctionByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(value: null);

        // act
        var result = await _auctionController.GetAuctionById(Guid.NewGuid());

        // assert
        Assert.IsType<NotFoundResult>(result.Result);
    }
}
