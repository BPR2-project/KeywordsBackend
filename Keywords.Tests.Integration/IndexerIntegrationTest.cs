using System.Net;
using System.Net.Http.Json;
using System.Web;
using indexer_api;
using Keywords.API.Swagger.Controllers.Generated;
using Keywords.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using Xunit;
using Xunit.Abstractions;

namespace Keywords.Tests.Integration;

public class IndexerIntegrationTest : IClassFixture<CustomWebApplicationFactory<Program>>, IDisposable
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly ITestOutputHelper _output;

    private readonly string TestVideoUrl = "gkjjkk";
    private readonly Guid TestVideoId = new("212ced62-0fda-4d02-b4e9-6d9b76122791");
    
    public IndexerIntegrationTest(CustomWebApplicationFactory<Program> factory, ITestOutputHelper output)
    {
        _factory = factory;
        _output = output;
    }
    
    public void Dispose()
    {
        _factory.Dispose();
    }

    [Fact]
    public async Task IndexerIntegrationTest_Indexer_ShouldIndexVideo()
    {
        _factory.IndexerMockServer
            .Given(Request.Create().UsingGet()
                .WithPath("/auth/trial/Accounts"))
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithBodyAsJson(new[] { new AccountInfo{ Id = "", Location = "", AccessToken = ""} }));

        _factory.IndexerMockServer
            .Given(Request.Create().UsingPost()
                .WithPath("/*/Accounts/*/Videos"))
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithBodyAsJson(new IndexVideoReceipt { Id = "" }));
        
        // Post video to indexer
        var client = _factory.CreateClient();

        var url = HttpUtility.UrlEncode(TestVideoUrl);
        var response = await client.PostAsync($"/indexer/{TestVideoId}?url={url}", null);
        if (response.StatusCode != HttpStatusCode.OK)
        {
            var content = await response.Content.ReadAsStringAsync();
            Assert.Fail($"Indexer failed to start: {response.StatusCode} - {content}");    
        }

        _factory.IndexerRepositoryMock
            .Verify(r => r.Insert(It.Is<IndexerEntity>(x => x.Id == TestVideoId), "email"),
                Times.Once);
        /*
          // Get indexer status
          IndexerProgress? progress;
          do
          {
              response = await client.GetAsync($"/indexer/{TestVideoId}");
              if (response.StatusCode != HttpStatusCode.OK)
              {
                  var content = await response.Content.ReadAsStringAsync();
                  Assert.Fail($"Failed to get Indexer status: {response.StatusCode} - {content}");
              }

              progress = await response.Content.ReadFromJsonAsync<IndexerProgress>();
              Assert.NotNull(progress);
              Assert.NotEqual(IndexerProgressState.Failed, progress.State);

              _output.WriteLine($"Indexer progress: {progress.State} - {progress.ProcessingProgress}");
              await Task.Delay(TimeSpan.FromSeconds(2));
          }
          while(progress.State != IndexerProgressState.Succeeded);
      */
    }
}