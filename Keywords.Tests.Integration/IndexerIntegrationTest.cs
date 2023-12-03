using System.Net;
using System.Net.Http.Json;
using System.Web;
using Keywords.API.Swagger.Controllers.Generated;
using Keywords.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Keywords.Tests.Integration;

public class IndexerIntegrationTest : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly ITestOutputHelper _output;

    private readonly string TestVideoUrl = "https://videoteststorageaccount.blob.core.windows.net/27663c85e1-input-53bd7/" +
                                           "cut.mp4?sp=r&st=2023-12-03T19:10:07Z&se=2024-02-01T03:10:07Z&spr=https&" +
                                           "sv=2022-11-02&sr=b&sig=%2Be1Ay2FPj2tWOB4l1Sjp7rpuIgGpRPRXbsQ0Pk7PLFc%3D";
    private readonly Guid TestVideoId = new("212ced62-0fda-4d02-b4e9-6d9b76122791");
    
    public IndexerIntegrationTest(WebApplicationFactory<Program> factory, ITestOutputHelper output)
    {
        _factory = factory;
        _output = output;
        CleanUp();
    }
    
    public void Dispose()
    {
        CleanUp();
        _factory.Dispose();
    }

    [Fact]
    public async Task IndexerIntegrationTest_Indexer_ShouldIndexVideo()
    {
        // Post video to indexer
        var client = _factory.CreateClient();

        var url = HttpUtility.UrlEncode(TestVideoUrl);
        var response = await client.PostAsync($"/indexer/{TestVideoId}?url={url}", null);
        if (response.StatusCode != HttpStatusCode.OK)
        {
            var content = await response.Content.ReadAsStringAsync();
            Assert.Fail($"Indexer failed to start: {response.StatusCode} - {content}");    
        }
        
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
    }
    
    private void CleanUp()
    {
        var scope = _factory.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<KeywordsContext>();

        var oldEntity = context.IndexerEntities.Find(TestVideoId);
        if (oldEntity != null)
        {
            context.IndexerEntities.Remove(oldEntity);
            context.SaveChanges();
        }
    }
}