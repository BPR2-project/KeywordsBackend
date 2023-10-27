using indexer_api;
using Keywords.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Keywords.Services;

public class IndexerService : IIndexerService
{
    private readonly IIndexerClient _indexerClient;
    private readonly string _apiKey;
    private readonly string _accountId;

    public IndexerService(IIndexerClient indexerClient, IConfiguration configuration)
    {
        _indexerClient = indexerClient;
        _apiKey = configuration["Indexer:ApiKey"];
        _accountId = configuration["Indexer:AccountId"];
    }
    
    public async Task<ICollection<Ocr>> GetKeywordsAsync(string videoId)
    {
        var accountInfos = await _indexerClient.GetTokenAsync(_apiKey);
        
        var accountInfo = accountInfos?.FirstOrDefault(x => x.Id == _accountId);
        if (accountInfo == null)
        {
            throw new Exception("No account found");
        }
        
        var response = await _indexerClient.GetIndexerOutputAsync(accountInfo.Location, accountInfo.Id, videoId, accountInfo.AccessToken);
        return response?.Videos?.FirstOrDefault()?.Insights.Ocr ?? Array.Empty<Ocr>();
    }
}