using AutoMapper;
using indexer_api;
using Keywords.API.Swagger.Controllers.Generated;
using Keywords.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Ocr = Keywords.API.Swagger.Controllers.Generated.Ocr;

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
    
    public async Task<ICollection<Ocr>> GetKeywordsAsync(string videoId)
    {
        var accountInfos = await _indexerClient.GetTokenAsync(_apiKey);
        
        var accountInfo = accountInfos?.FirstOrDefault(x => x.Id == _accountId);
        if (accountInfo == null)
        {
            throw new Exception("No account found");
        }
        var response = await _indexerClient.GetIndexerOutputAsync(accountInfo.Location, accountInfo.Id, videoId, accountInfo.AccessToken);
        var toMap = response?.Videos?.FirstOrDefault()?.Insights.Ocr;

        if (toMap == null)
        {
            throw new Exception("No Indexer output found");
        }
        
        return _mapper.Map<ICollection<Ocr>>(toMap);
    }
}