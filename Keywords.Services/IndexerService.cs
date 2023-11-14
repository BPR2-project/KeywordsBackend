using System.Net.Http.Json;
using AutoMapper;
using indexer_api;
using Keywords.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using RequestVideoIndexResponse = Keywords.API.Swagger.Controllers.Generated.RequestVideoIndexResponse;
using Video = Keywords.API.Swagger.Controllers.Generated.Video;

namespace Keywords.Services;

public class IndexerService : IIndexerService
{
    private readonly IIndexerClient _indexerClient;
    private readonly string _apiKey;
    private readonly string _accountId;
    private readonly IMapper _mapper;


    public IndexerService(IIndexerClient indexerClient, IConfiguration configuration, IMapper mapper)
    {
        _indexerClient = indexerClient;
        _mapper = mapper;
        _apiKey = configuration["Indexer:ApiKey"];
        _accountId = configuration["Indexer:AccountId"];
    }
    
    public async Task<ICollection<Video>> GetIndexerOutputAsync(string videoId)
    {
        var accountInfos = await _indexerClient.GetTokenAsync(_apiKey);
        
        var accountInfo = accountInfos?.FirstOrDefault(x => x.Id == _accountId);
        if (accountInfo == null)
        {
            throw new Exception("No account found");
        }
        var response = await _indexerClient.GetIndexerOutputAsync(accountInfo.Location, accountInfo.Id, videoId, accountInfo.AccessToken);
        var toMap = response?.Videos;

        if (toMap == null)
        {
            throw new Exception("No Indexer output found");
        }

        if (toMap.Any(x => x.Insights?.Ocr == null))
        {
            return _mapper.Map<ICollection<Video>>(toMap);
        }
        
        foreach (var video in toMap)
        {
            video.Insights.Ocr = video.Insights.Ocr
                .Where(x => x.Confidence >= 0.96 && (x.Language == "da-DK" || x.Text.ToLower().IndexOfAny(new[] { 'æ', 'ø', 'å' }) > -1))
                .OrderByDescending(x => x.Instances.Count).Take(50).ToList();
        }
        
        return _mapper.Map<ICollection<Video>>(toMap);
    }

    public async Task<RequestVideoIndexResponse> IndexVideoAsync(string url, string videoName, string description)
    {
        var accountInfos = await _indexerClient.GetTokenAsync(_apiKey);
        var accountInfo = accountInfos?.FirstOrDefault(x => x.Id == _accountId);
        if (accountInfo == null)
        {
            throw new Exception("No account found");
        }

        try
        {
            var response = await _indexerClient.IndexVideoAsync(accountInfo.Location, accountInfo.Id,
                accountInfo.AccessToken, videoName, url, description, "private", "partition", "VideoOnly","en-US,da-DK");
            return _mapper.Map<RequestVideoIndexResponse>(response);
        }
        catch (ApiException<IndexInProgress> e)
        {
            return _mapper.Map<RequestVideoIndexResponse>(e.Result);
        }
    }

}