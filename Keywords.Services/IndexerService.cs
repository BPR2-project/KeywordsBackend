using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;
using AutoMapper;
using indexer_api;
using Keywords.Data;
using Keywords.Data.Repositories.Interfaces;
using Keywords.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using VideoIndexerResponse = Keywords.API.Swagger.Controllers.Generated.VideoIndexerResponse;


namespace Keywords.Services;

public class IndexerService : IIndexerService
{
    private readonly IIndexerClient _indexerClient;
    private readonly string _apiKey;
    private readonly string _accountId;
    private readonly IMapper _mapper;
    private readonly IIndexerEntityRepository _indexerEntityRepository;

    public IndexerService(IIndexerClient indexerClient, IConfiguration configuration, IMapper mapper, IIndexerEntityRepository indexerEntityRepository)
    {
        _indexerClient = indexerClient;
        _mapper = mapper;
        _indexerEntityRepository = indexerEntityRepository;
        _apiKey = configuration["Indexer:ApiKey"];
        _accountId = configuration["Indexer:AccountId"];
    }

    public Task<VideoIndexerResponse> GetIndexerOutputAsync(Guid videoId)
    {
        throw new NotImplementedException();
    }

/*
    public async Task<VideoIndexerResponse> GetIndexerOutputAsync(Guid videoId)
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
            return _mapper.Map<>(toMap);
        }

        var ocr = toMap.SelectMany(video => video.Insights.Ocr, (_, t) => t.Text).ToList();
        var ocrDoc = new { id = 1, text = string.Join(". ", ocr), language = "da" };
        
        var request = new
        {
            analysisInput = new { documents = new[] { ocrDoc } },
            tasks = new[] { new { kind = "KeyPhraseExtraction", parameters = new { modelVersion = "latest" } } }
        };
        
        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "362e6105acc6405585aade0f4305e292");
        var analyzeResult = await client.PostAsJsonAsync("https://languageresource123.cognitiveservices.azure.com/language/analyze-text/jobs?api-version=2022-05-01", request);
        analyzeResult.EnsureSuccessStatusCode();
        
        var location = analyzeResult.Headers.FirstOrDefault(x => x.Key == "operation-location").Value?.FirstOrDefault();

        var blackList = new [] {"modul", "lektion", "ord", "allan", "eksempel", "tror", "næste", "swap lang", "danmark"};

        while (true)
        {
            var result = await client.GetAsync(location);
            result.EnsureSuccessStatusCode();
        
            var json = await result.Content.ReadAsStringAsync();
            var analyzeJobResult = JsonSerializer.Deserialize<AnalyzeJobResult>(json);
            
            if (analyzeJobResult?.status == "succeeded")
            {
                var keyPhrases = analyzeJobResult.tasks.items
                    .SelectMany(x => x.results.documents.SelectMany(y => y.keyPhrases).ToList()).ToList();

                File.WriteAllText("C:\\Users\\agosm\\Keywords discovery\\keyPhrases_from_ocr.json", JsonSerializer.Serialize(keyPhrases));

                var topPhrases = keyPhrases.
                    Where(x => !blackList.Contains(x.ToLower()))
                    .OrderByDescending(x =>
                        Regex.Matches(ocrDoc.text, x, RegexOptions.IgnoreCase).Count)
                    .Take(15);

                File.WriteAllText("C:\\Users\\agosm\\Keywords discovery\\topPhrases_from_ocr.json", JsonSerializer.Serialize(topPhrases));
                break;
            }
        }
       
        return _mapper.Map<ICollection<Video>>(toMap);
    }

    record AnalyzeJobResult(string status, Tasks tasks);
    record Tasks(Item[] items);
    record Item(Results results);
    record Results(Documents[] documents);
    record Documents(string id, string[] keyPhrases);

*/
    public async Task IndexVideoAsync(Guid videoId, string url)
    {
        var accountInfos = await _indexerClient.GetTokenAsync(_apiKey);
        var accountInfo = accountInfos?.FirstOrDefault(x => x.Id == _accountId);
        if (accountInfo == null)
        {
            throw new Exception("No account found");
        }

        var videoName = videoId.ToString();
        var response = await _indexerClient.IndexVideoAsync(accountInfo.Location, accountInfo.Id,
            accountInfo.AccessToken, videoName, url, "description", "private", "partition",
            new[] { "Faces", "ObservedPeople", "Emotions", "Labels" }, "Default", "en-US,da-DK");
      
        if (response == null)
        {
            throw new Exception("No Indexer output found");
        }
        
        _indexerEntityRepository.Insert(new IndexerEntity { VideoId = videoId, State = "Indexing", IndexerId = response.Id}, "indexer.service@email.com");
        _indexerEntityRepository.Save();
    }
}