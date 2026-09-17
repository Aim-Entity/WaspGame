using Application.Abstracts;
using Application.Abstracts.Repositories;
using Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;
using Moq;

namespace Application.UnitTests.Configuration;

public class ConfigureServiceTests
{
    private static ServiceCollection CreateServicesWithARepository()
    {
        var services = new ServiceCollection();
        services.AddSingleton(new Mock<IGameRepository>().Object);

        return services;
    }

    [Fact]
    public void ConfigureService_AddApplicationServices_WhenWiringUpTheContainer_ResolvesTheGameServiceAsTheGameServiceAbstraction()
    {
        // ARRANGE
        var services = CreateServicesWithARepository();

        // ACT
        using var provider = services.AddApplicationServices().BuildServiceProvider();
        var gameService = provider.GetRequiredService<IGameService>();

        // ASSERT
        Assert.IsType<GameService>(gameService);
    }

    [Fact]
    public void ConfigureService_AddApplicationServices_WhenWiringUpTheContainer_RegistersTheGameServiceAsScoped()
    {
        // ARRANGE
        var services = CreateServicesWithARepository();

        // ACT
        var descriptor = services.AddApplicationServices()
            .Single(d => d.ServiceType == typeof(IGameService));

        // ASSERT
        Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
        Assert.Equal(typeof(GameService), descriptor.ImplementationType);
    }

    [Fact]
    public void ConfigureService_AddApplicationServices_WhenNoTimeProviderIsRegistered_FallsBackToTheSystemClock()
    {
        // ARRANGE
        var services = CreateServicesWithARepository();

        // ACT
        using var provider = services.AddApplicationServices().BuildServiceProvider();
        var timeProvider = provider.GetRequiredService<TimeProvider>();

        // ASSERT
        Assert.Same(TimeProvider.System, timeProvider);
    }

    [Fact]
    public void ConfigureService_AddApplicationServices_WhenNoRandomIsRegistered_FallsBackToTheSharedRandom()
    {
        // ARRANGE
        var services = CreateServicesWithARepository();

        // ACT
        using var provider = services.AddApplicationServices().BuildServiceProvider();
        var random = provider.GetRequiredService<Random>();

        // ASSERT
        Assert.Same(Random.Shared, random);
    }
}
