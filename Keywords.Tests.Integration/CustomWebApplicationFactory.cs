using AutoFixture;
using AutoFixture.AutoMoq;
using Keywords.Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using WireMock.Server;

namespace Keywords.Tests.Integration;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public readonly Mock<IKeywordEntityRepository> KeywordsRepositoryMock = new();
    public readonly Mock<IIndexerEntityRepository> IndexerRepositoryMock = new();
    public readonly WireMockServer IndexerMockServer = WireMockServer.Start();
    public readonly WireMockServer KeyPhraseMockServer = WireMockServer.Start();

    public readonly string IndexerAccountId;
    public readonly string KeyPhraseApiKey;
    
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
    private T A<T>() => _fixture.Create<T>();
    
    public CustomWebApplicationFactory()
    {
        IndexerAccountId = A<string>();
        KeyPhraseApiKey = A<string>();
    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var configValues = new Dictionary<string, string?>
        {
            { "ConnectionStrings:KeywordsDb", "Won't be used because we are mocking the repositories" },
            { "Indexer:BaseUrl", IndexerMockServer.Url },
            { "Indexer:AccountId", IndexerAccountId },
            { "KeyPhrase:BaseUrl", KeyPhraseMockServer.Url },
            { "KeyPhrase:ApiKey", KeyPhraseApiKey }
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