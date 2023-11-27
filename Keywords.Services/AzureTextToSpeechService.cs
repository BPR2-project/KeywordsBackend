using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using Keywords.API.Swagger.Controllers.Generated;
using Keywords.Data;
using Keywords.Data.Repositories.Interfaces;
using Keywords.Extensions;
using Keywords.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using textToSpeech_api;
using Microsoft.CognitiveServices.Speech;

namespace Keywords.Services;

public class AzureTextToSpeechService : IAzureTextToSpeechService
{
    private IKeywordEntityRepository _keywordEntityRepository;
    private IAzureTextToSpeechClient _azureTextToSpeechClient;
    private string _ttsSubscriptionKey;
    private string _ttsRegion;
    private string _blobUri;
    private string _blobContainer;

    public AzureTextToSpeechService(IAzureTextToSpeechClient azureTextToSpeechClient,
        IKeywordEntityRepository keywordEntityRepository, IConfiguration configuration)
    {
        _blobUri = configuration["BlobStorage:Uri"];
        _blobContainer = configuration["BlobStorage:Container"];
        _ttsSubscriptionKey = configuration["TextToSpeech:Key"];
        _ttsRegion = configuration["Azure:Region"];
        _azureTextToSpeechClient = azureTextToSpeechClient;
        _keywordEntityRepository = keywordEntityRepository;
    }
    
    public async Task ConvertTextToSpeech(Guid videoId)
    {
        var voicesList = await _azureTextToSpeechClient.GetAllVoicesAsync(_ttsSubscriptionKey);

        var videoKeywords = _keywordEntityRepository.GetAllKeywordsByVideoId(videoId, int.MaxValue, 0);

        if (videoKeywords.keywords == null || videoKeywords.keywords.Count == 0)
            return;

        var keywordsLanguage = videoKeywords.keywords.First().Language;

        var speechConfig = SpeechConfig.FromSubscription(_ttsSubscriptionKey, _ttsRegion);
        speechConfig.SpeechSynthesisVoiceName = voicesList
            .First(a => a.Locale == keywordsLanguage && a.Gender == "Female").ShortName;

        var containerClient = new BlobContainerClient(_blobUri, _blobContainer);

        using (var synthesizer = new SpeechSynthesizer(speechConfig, null))
        {
            var currentKeyword = new KeywordEntity();

            var synthesisCompleted = false;

            synthesizer.SynthesisCompleted += async (o, e) =>
            {
                synthesisCompleted = true;
                var audioBuffer = e.Result.AudioData;

                MemoryStream audioStream = new MemoryStream(audioBuffer);
                var blobClient = containerClient.GetBlobClient($"{currentKeyword.Content}.wav");

                BlobHttpHeaders blobHttpHeaders = new BlobHttpHeaders()
                {
                    ContentType = "audio/wav"
                };

                try
                {
                    if (!await blobClient.ExistsAsync())
                        await blobClient.UploadAsync(audioStream, blobHttpHeaders);
                    
                    var audioSasLink = await CreateServiceSASBlob(blobClient);

                    currentKeyword.AudioLink = audioSasLink.ToString();
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    throw;
                }

                _keywordEntityRepository.Update(currentKeyword, "system");
                _keywordEntityRepository.SaveAndStopTracking();
            };

            synthesizer.SynthesisCanceled += (o, e) =>
            {
                SpeechSynthesisCancellationDetails cancellation =
                    SpeechSynthesisCancellationDetails.FromResult(e.Result);

                throw new Exception(cancellation.ErrorDetails);
            };

            foreach (var keyword in videoKeywords.keywords)
            {
                currentKeyword = keyword;

                synthesisCompleted = false;

                await synthesizer.SpeakTextAsync(keyword.Content);

                while (!synthesisCompleted)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            }
        }
    }

    private async Task<Uri> CreateServiceSASBlob(
        BlobClient blobClient,
        string storedPolicyName = null)
    {
        // Check if BlobContainerClient object has been authorized with Shared Key
        if (blobClient.CanGenerateSasUri)
        {
            // Create a SAS token that's valid for one day
            BlobSasBuilder sasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = blobClient.GetParentBlobContainerClient().Name,
                BlobName = blobClient.Name,
                Resource = "b"
            };

            if (storedPolicyName == null)
            {
                sasBuilder.ExpiresOn = DateTimeOffset.UtcNow.AddMonths(3);
                sasBuilder.SetPermissions(BlobContainerSasPermissions.Read);
            }
            else
            {
                sasBuilder.Identifier = storedPolicyName;
            }

            Uri sasURI = blobClient.GenerateSasUri(sasBuilder);

            return sasURI;
        }
        else
        {
            // Client object is not authorized via Shared Key
            return null;
        }
    }
}