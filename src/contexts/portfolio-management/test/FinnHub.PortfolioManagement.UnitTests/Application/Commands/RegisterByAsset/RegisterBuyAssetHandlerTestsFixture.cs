using FinnHub.PortfolioManagement.Application.Abstractions;
using FinnHub.PortfolioManagement.Application.Abstractions.MarketData;
using FinnHub.PortfolioManagement.Application.Abstractions.Users;
using FinnHub.PortfolioManagement.Application.Commands.RegisterBuyAsset;
using FinnHub.PortfolioManagement.Domain.Aggregates;
using FinnHub.PortfolioManagement.Domain.Aggregates.Repositories;
using FinnHub.PortfolioManagement.UnitTests.Application.Commands.Common;
using FinnHub.PortfolioManagement.UnitTests.Application.Common;

using NSubstitute;

namespace FinnHub.PortfolioManagement.UnitTests.Application.Commands.RegisterByAsset;
public sealed class RegisterBuyAssetHandlerTestsFixture : CommandsBaseFixture
{
    public IPortfolioRepository PortfolioRepository = Substitute.For<IPortfolioRepository>();
    public IUnitOfWork UnitOfWork = Substitute.For<IUnitOfWork>();
    public IUserContext UserContext = Substitute.For<IUserContext>();
    public IMarketDataService MarketDataService = Substitute.For<IMarketDataService>();

    public Portfolio Portfolio = new PortfolioBuilder().Build();

    public RegisterBuyAssetRequest GetValidRequest() => new()
    {
        PortfolioId = Guid.NewGuid(),
        AssetSymbol = Faker.Random.String(7),
        PricePerUnit = Faker.Random.Decimal(1, 100),
        Quantity = Faker.Random.Int(1, 100),
        TransactionDate = DateTimeOffset.UtcNow
    };
}
