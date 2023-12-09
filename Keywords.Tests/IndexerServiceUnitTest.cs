using FluentAssertions;
using indexer_api;
using Keywords.Data;
using Keywords.Data.Repositories.Interfaces;
using Keywords.Services;
using Keywords.Tests.Integration;
using Moq;
using Xunit;

namespace Keywords.Tests;

public class IndexerServiceUnitTest : TestFixture
{
    [Fact]
    public void Can_Create_Indexer_Entity()
    {
        var repository = Mock<IIndexerEntityRepository>();
        var videoId = A<Guid>();
        var response = A<IndexVideoReceipt>();
        var sut = A<IndexerService>();

        sut.CreateIndexerEntity(videoId, response);

        repository.Verify(r => r.Insert(It.Is<IndexerEntity>(x => x.Id == videoId), "email"), Times.Once);
        repository.Verify(r => r.Save(), Times.Once);
    }
    
    [Fact]
    public void Entity_Can_Be_Updated_To_ExtractingKeyPhrases()
    {
        var repository = Mock<IIndexerEntityRepository>();
        var entity = A<IndexerEntity>();
        entity.State = IndexerState.Indexing;
        var job = A<string>();
        var sut = A<IndexerService>();
        
        sut.UpdateToExtractKeyPhrases(entity, job);
        
        repository.Verify(r => r.Update(entity, "email"), Times.Once);
        repository.Verify(r => r.Save(), Times.Once);
        entity.State.Should().Be(IndexerState.ExtractingKeyPhrases);
        entity.KeyPhraseJobId.Should().Be(job);
    }
    
    [Fact]
    public void Can_Update_Entity_To_Succeeded()
    {
        var indexerRepository= Mock<IIndexerEntityRepository>();
        var keywordsRepository = Mock<IKeywordEntityRepository>();
        var entity = A<IndexerEntity>();
        entity.State = IndexerState.ExtractingKeyPhrases;
        var sut = A<IndexerService>();
        var intersection = A<List<string>>();
        
        sut.UpdateToSucceeded(entity, intersection);
        
        keywordsRepository.Verify(r=> r.Save(), Times.Once);
        indexerRepository.Verify(r => r.Update(entity, "email"), Times.Once);
        indexerRepository.Verify(r=> r.Save(), Times.Once);
        entity.State.Should().Be(IndexerState.Succeeded);
    }
    
    [Fact]
    public void Can_Update_Entity_To_Failed()
    {
        var indexerRepository= Mock<IIndexerEntityRepository>();
        var keywordsRepository = Mock<IKeywordEntityRepository>();
        var entity = A<IndexerEntity>();
        entity.State = IndexerState.ExtractingKeyPhrases;
        var sut = A<IndexerService>();
        var intersection = new List<string>();

        
        sut.UpdateToFailed(entity);
        
        indexerRepository.Verify(r => r.Update(entity, "email"), Times.Once);
        indexerRepository.Verify(r=> r.Save(), Times.Once);
        entity.State.Should().Be(IndexerState.Failed);
    }


}