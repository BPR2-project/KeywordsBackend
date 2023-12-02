using System.Net;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using System.Web;
using AutoMapper;
using indexer_api;
using Keywords.API.Swagger.Controllers.Generated;
using Keywords.Data;
using Keywords.Data.Repositories.Interfaces;
using Keywords.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Keywords.Services;

public class IndexerService : IIndexerService
{
    private readonly IIndexerClient _indexerClient;
    private readonly IKeyPhraseClient _keyPhraseClient;
    private readonly IIndexerEntityRepository _indexerEntityRepository;
    private readonly IKeywordEntityRepository _keywordEntityRepository;
    private readonly IMapper _mapper;

    private readonly string _indexerApiKey;
    private readonly string _indexerAccountId;
    private readonly string _keyPhraseApiKey;

    public IndexerService(IIndexerClient indexerClient, IKeyPhraseClient keyPhraseClient,
        IIndexerEntityRepository indexerEntityRepository, IKeywordEntityRepository keywordEntityRepository,
        IMapper mapper, IConfiguration configuration)
    {
        _indexerClient = indexerClient;
        _keyPhraseClient = keyPhraseClient;
        _indexerEntityRepository = indexerEntityRepository;
        _keywordEntityRepository = keywordEntityRepository;
        _mapper = mapper;

        _indexerApiKey = configuration["Indexer:ApiKey"];
        _indexerAccountId = configuration["Indexer:AccountId"];
        _keyPhraseApiKey = configuration["KeyPhrase:ApiKey"];
    }

    public async Task<IndexerResponse?> GetIndexerOutputAsync(Guid videoId)
    {
        var entity = _indexerEntityRepository.GetById(videoId);
        if (entity == null) return null;

        switch (entity.State)
        {
            case IndexerState.Indexing:
                return await GetIndexerResultAsync(entity);
            case IndexerState.ExtractingKeyPhrases:
                return await GetKeyPhraseResultAsync(entity);
            case IndexerState.Succeeded:
                return GetKeyWords(entity);
            case IndexerState.Failed:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return _mapper.Map<IndexerResponse>(entity);
    }

    private IndexerResponse GetKeyWords(IndexerEntity entity)
    {
        var keywords = _keywordEntityRepository.GetAllKeywordsByVideoId(entity.Id, 100, 0).keywords;
        return _mapper.Map<IndexerResponse>(keywords);
    }

    private async Task<IndexerResponse?> GetKeyPhraseResultAsync(IndexerEntity entity)
    {
        var response = await _keyPhraseClient.GetJobResultAsync(_keyPhraseApiKey, entity.KeyPhraseJobId);
        if (response.Result?.Status != "succeeded")
        {
            return _mapper.Map<IndexerResponse>(entity);
        }

        var documents = response.Result.Tasks?.Items?.FirstOrDefault()?.Results?.Documents?.ToList();
        if (documents?.Count != 2)
        {
            throw new Exception("Unexpected number of documents");
        }

        var ocrKeyPhrases = documents[0].KeyPhrases;
        var transcriptKeyPhrases = documents[1].KeyPhrases;
        var keyPhraseIntersection = transcriptKeyPhrases.OrderByDescending(trans =>
                ocrKeyPhrases.Count(ocr => string.Equals(ocr, trans, StringComparison.InvariantCultureIgnoreCase)))
            .Take(50).ToList();
        
        keyPhraseIntersection = keyPhraseIntersection.ConvertAll(text => Regex.Replace(text, "^[a-z]", c => c.Value.ToUpper()));

        var keywords = keyPhraseIntersection.Select(x => new KeywordEntity
        {
            Content = x,
            VideoId = entity.Id,
            Language = "da-DK",
            AudioLink = ""
        }).ToList();

        _keywordEntityRepository.InsertRange(keywords, "email");
        _keywordEntityRepository.Save();

        entity.State = IndexerState.Succeeded;
        _indexerEntityRepository.Update(entity, "email");
        _indexerEntityRepository.Save();

        return GetKeyWords(entity);
    }

    private async Task<IndexerResponse> GetIndexerResultAsync(IndexerEntity entity)
    {
        var video = await GetIndexerOutputAsync(entity);
        if (video.Insights?.Ocr == null || video.Insights?.Transcript == null)
        {
            return _mapper.Map<IndexerResponse>(video);
        }

        var job = await CreateJobRequest(video);

        entity.State = IndexerState.ExtractingKeyPhrases;
        entity.KeyPhraseJobId = job;

        _indexerEntityRepository.Update(entity, "email");
        _indexerEntityRepository.Save();

        return _mapper.Map<IndexerResponse>(entity);
    }

    private async Task<string?> CreateJobRequest(Video video)
    {
        var request = CreateKeyPhraseRequest(video);
        var jobResponse = await _keyPhraseClient.CreateJobAsync(_keyPhraseApiKey, request);

        if (!jobResponse.Headers.TryGetValue("operation-location", out var locations) ||
            !Uri.TryCreate(locations?.FirstOrDefault() ?? "", UriKind.Absolute, out var uri))
        {
            throw new Exception("Could not get operation location");
        }

        return Path.GetFileName(uri.AbsolutePath);
    }

    private static KeyPhraseRequest CreateKeyPhraseRequest(Video video)
    {
        var ocr = video.Insights.Ocr.Select(x => x.Text).ToList().ConvertAll(text => text.ToLower());
        var transcript = video.Insights.Transcript.Select(x => x.Text).ToList().ConvertAll(text => text.ToLower());

        var request = new KeyPhraseRequest
        {
            AnalysisInput = new AnalysisInput
            {
                Documents = new[]
                {
                    new Document { Id = 1, Text = string.Join(" ", ocr), Language = "da" },
                    new Document { Id = 2, Text = string.Join(". ", transcript), Language = "da" }
                }
            },
            Tasks = new[]
            {
                new JobTask
                {
                    Kind = JobTaskKind.KeyPhraseExtraction,
                    Parameters = new Parameters { ModelVersion = ParametersModelVersion.Latest }
                }
            }
        };
        return request;
    }

    private async Task<Video> GetIndexerOutputAsync(IndexerEntity entity)
    {
        var accountInfo = await GetAccountInfoAsync();
        var response = await _indexerClient.GetIndexerOutputAsync(accountInfo.Location, accountInfo.Id,
            entity.IndexerId,
            accountInfo.AccessToken);
        var video = response?.Videos?.FirstOrDefault();

        if (video == null)
        {
            throw new Exception("No Indexer output found");
        }

        return video;
    }

    public async Task IndexVideoAsync(Guid videoId, string url)
    {
        var accountInfo = await GetAccountInfoAsync();

        var videoName = videoId.ToString();
        var response = await _indexerClient.IndexVideoAsync(accountInfo.Location, accountInfo.Id,
            accountInfo.AccessToken, videoName, url, "private", "da-DK", "da-DK",
            new[] { "Faces", "ObservedPeople", "Emotions", "Labels" });

        if (response == null)
        {
            throw new Exception("No Indexer output found");
        }

        _indexerEntityRepository.Insert(
            new IndexerEntity { Id = videoId, State = IndexerState.Indexing, IndexerId = response.Id },
            "email");
        _indexerEntityRepository.Save();
    }

    private async Task<AccountInfo> GetAccountInfoAsync()
    {
        var accountInfos = await _indexerClient.GetTokenAsync(_indexerApiKey);

        var accountInfo = accountInfos?.FirstOrDefault(x => x.Id == _indexerAccountId);
        return accountInfo ?? throw new Exception("No account found");
    }
}