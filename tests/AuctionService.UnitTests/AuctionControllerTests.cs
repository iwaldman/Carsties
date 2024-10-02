using AuctionService.Controllers;
using AuctionService.Data;
using AuctionService.RequestHelpers;
using AutoFixture;
using AutoMapper;
using MassTransit;
using Moq;

namespace AuctionService.UnitTests;

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
}
