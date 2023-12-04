using Keywords.Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using WireMock.Server;

namespace Keywords.Tests.Integration;

public class CustomWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
{
    public readonly Mock<IKeywordEntityRepository> KeywordsRepositoryMock = new();
    public readonly Mock<IIndexerEntityRepository> IndexerRepositoryMock = new();
    public readonly WireMockServer IndexerMockServer = WireMockServer.Start();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var configValues = new Dictionary<string, string>
        {
            { "ConnectionStrings:KeywordsDb", "Value" },
            { "Indexer:BaseUrl", IndexerMockServer.Url! },
            { "Indexer:AccountId", "" }
        };
  
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configValues)
            .Build();

        builder
            .UseConfiguration(configuration)
            .ConfigureAppConfiguration(configurationBuilder =>
            {
                configurationBuilder.AddInMemoryCollection(configValues);
            })
            .ConfigureServices(services =>
            {
                services.Remove(services.Single(d => d.ServiceType == typeof(IKeywordEntityRepository)));
                services.AddSingleton(KeywordsRepositoryMock.Object);
                
                services.Remove(services.Single(d => d.ServiceType == typeof(IIndexerEntityRepository)));
                services.AddSingleton(IndexerRepositoryMock.Object);
            });

        builder.UseEnvironment("Development");
    }
}