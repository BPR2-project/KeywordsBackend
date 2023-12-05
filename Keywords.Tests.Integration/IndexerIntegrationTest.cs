using System.Net;
using System.Net.Http.Json;
using AutoFixture;
using indexer_api;
using Keywords.API.Swagger.Controllers.Generated;
using Keywords.Data;
using Moq;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using Xunit;

namespace Keywords.Tests.Integration;

public class IndexerIntegrationTest : TestFixture, IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    
    public IndexerIntegrationTest(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }
    
    [Fact]
    public async Task PostIndexVideo_ShouldBeSuccessful()
    {
        // Arrange
        var testVideoUrl = A<Uri>();
        var testVideoId = A<Guid>();
        var accountInfo = ArrangeAccountInfo();
        ArrangePostVideoToIndexer(accountInfo);

        // Act
        var client = _factory.CreateClient();
        var response = await client.PostAsync($"/indexer/{testVideoId}?url={testVideoUrl}", null);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        _factory.IndexerRepositoryMock.Verify(r => r.Insert(It.Is<IndexerEntity>(x => x.Id == testVideoId), "email"),
            Times.Once);
    }

    [Fact]
    public async Task GetIndexVideoProgress_Successful()
    {
        // Arrange
        var testVideoId = A<Guid>();
        var entity = ArrangeRepository(testVideoId, IndexerState.Indexing);
        var accountInfo = ArrangeAccountInfo();
        var video = ArrangeGetIndexerResponse(entity.IndexerId, accountInfo);
        video.Insights.Transcript = null;

        // Act
        var client = _factory.CreateClient();
        var response = await client.GetAsync($"/indexer/{testVideoId}");
        var progress = await response.Content.ReadFromJsonAsync<IndexerProgress>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(IndexerProgressState.Indexing, progress?.State);
        Assert.Equal(video.ProcessingProgress, progress?.ProcessingProgress);
        _factory.IndexerRepositoryMock.Verify(r => r.Insert(It.IsAny<IndexerEntity>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task PostJobRequestToKeyPhrase_Successful()
    {
        // Arrange
        var testVideoId = A<Guid>();
        var entity = ArrangeRepository(testVideoId, IndexerState.Indexing);
        var accountInfo = ArrangeAccountInfo();
        ArrangeGetIndexerResponse(entity.IndexerId, accountInfo);
        ArrangePostKeyPhraseJob();

        // Act
        var client = _factory.CreateClient();
        var response = await client.GetAsync($"/indexer/{testVideoId}");
        var progress = await response.Content.ReadFromJsonAsync<IndexerProgress>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(IndexerProgressState.ExtractingKeyPhrases, progress?.State);
        Assert.Null(progress?.ProcessingProgress);
        _factory.IndexerRepositoryMock.Verify(r => r.Update(It.Is<IndexerEntity>(x => x.Id == testVideoId), "email"),
            Times.Once);
    }

    [Fact]
    public async Task GetJobResultFromKeyPhrase_Successful()
    {
        // Arrange
        var testVideoId = A<Guid>();
        var entity = ArrangeRepository(testVideoId, IndexerState.ExtractingKeyPhrases);
        ArrangeGetKeyPhraseResult(entity.KeyPhraseJobId);

        // Act
        var client = _factory.CreateClient();
        var response = await client.GetAsync($"/indexer/{testVideoId}");
        var progress = await response.Content.ReadFromJsonAsync<IndexerProgress>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(IndexerProgressState.Succeeded, progress?.State);
        Assert.Null(progress?.ProcessingProgress);
        _factory.KeywordsRepositoryMock.Verify(r => r.InsertRange(It.IsAny<IEnumerable<KeywordEntity>>(), "email"), 
            Times.Once);
        _factory.IndexerRepositoryMock.Verify(r => r.Update(It.Is<IndexerEntity>(x => x.Id == testVideoId), "email"),
            Times.Once);
    }
    
    private IndexerEntity ArrangeRepository(Guid testVideoId, IndexerState state)
    {
        var indexerEntity = Build<IndexerEntity>().With(x => x.State, state)
            .With(x => x.Id, testVideoId).Create();
        _factory.IndexerRepositoryMock.Setup(r => r.GetById(testVideoId))
            .Returns(indexerEntity);
        return indexerEntity;
    }

    private AccountInfo ArrangeAccountInfo()
    {
        var accountInfo = Build<AccountInfo>()
            .With(x => x.Id, _factory.IndexerAccountId).Create();
        
        _factory.IndexerMockServer
            .Given(Request.Create().UsingGet()
                .WithPath("/auth/trial/Accounts"))
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithBodyAsJson(new[] { accountInfo }));

        return accountInfo;
    }

    private void ArrangePostVideoToIndexer(AccountInfo accountInfo)
    {
        _factory.IndexerMockServer
            .Given(Request.Create().UsingPost()
                .WithPath($"/{accountInfo.Location}/Accounts/{accountInfo.Id}/Videos"))
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithBodyAsJson(Build<IndexVideoReceipt>().Create()));
    }
    
    private Video ArrangeGetIndexerResponse(string indexerId, AccountInfo accountInfo)
    {
        var video = A<Video>();

        var indexerResponse = Build<VideoIndexerResponse>()
            .With(x => x.Videos, new[] { video })
            .Create();
        
        _factory.IndexerMockServer.Given(Request.Create().UsingGet()
                .WithPath($"/{accountInfo.Location}/Accounts/{accountInfo.Id}/Videos/{indexerId}/Index"))
            .RespondWith(Response.Create()
                .WithSuccess().WithBodyAsJson(indexerResponse));

        return video;
    }
    
    private void ArrangePostKeyPhraseJob()
    {
        var operationLocation = $"{A<Uri>()}/{A<string>()}";
        _factory.KeyPhraseMockServer.Given(Request.Create().UsingPost()
                .WithPath("/language/analyze-text/jobs"))
            .RespondWith(Response.Create().WithStatusCode(HttpStatusCode.Accepted)
                .WithHeader("operation-location", operationLocation));
    }
    
    private void ArrangeGetKeyPhraseResult(string? jobId)
    {
        var jobResult = A<JobResult>();
        jobResult.Status = "succeeded";
        var results = jobResult.Tasks.Items.First().Results;
        results.Documents = results.Documents.Take(2).ToList();
        
        _factory.KeyPhraseMockServer.Given(Request.Create().UsingGet()
                .WithPath($"/language/analyze-text/jobs/{jobId}"))
            .RespondWith(Response.Create().WithStatusCode(HttpStatusCode.OK).WithBodyAsJson(jobResult));
    }
}
