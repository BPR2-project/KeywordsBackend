using AutoMapper;
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
    private IMapper _mapper;
    private string _ttsSubscriptionKey;
    private string _ttsRegion;
    private string _blobUri;
    private string _blobContainer;

    public AzureTextToSpeechService(IAzureTextToSpeechClient azureTextToSpeechClient,
        IKeywordEntityRepository keywordEntityRepository, IConfiguration configuration, IMapper mapper)
    {
        _blobUri = configuration["BlobStorage:Uri"];
        _blobContainer = configuration["BlobStorage:Container"];
        _ttsSubscriptionKey = configuration["TextToSpeech:Key"];
        _ttsRegion = configuration["Azure:Region"];
        _azureTextToSpeechClient = azureTextToSpeechClient;
        _keywordEntityRepository = keywordEntityRepository;
        _mapper = mapper;
    }
    
    public async Task<Keyword> CreateAudio(Guid keywordId)
    {
        var voicesList = await _azureTextToSpeechClient.GetAllVoicesAsync(_ttsSubscriptionKey);

        var keyword = _keywordEntityRepository.GetById(keywordId);

        var keywordLanguage = keyword.Language;

        var speechConfig = SpeechConfig.FromSubscription(_ttsSubscriptionKey, _ttsRegion);
        speechConfig.SpeechSynthesisVoiceName = voicesList
            .First(a => a.Locale == keywordLanguage && a.Gender == "Female").ShortName;

        var containerClient = new BlobContainerClient(_blobUri, _blobContainer);

        using (var synthesizer = new SpeechSynthesizer(speechConfig, null))
        {
            await synthesizer.SpeakTextAsync(keyword.Content);

            synthesizer.SynthesisCompleted += async (o, e) =>
            {
                var audioBuffer = e.Result.AudioData;

                MemoryStream audioStream = new MemoryStream(audioBuffer);
                var blobClient = containerClient.GetBlobClient($"{keyword.Content}.wav");

                BlobHttpHeaders blobHttpHeaders = new BlobHttpHeaders()
                {
                    ContentType = "audio/wav"
                };

                try
                {
                    if (!await blobClient.ExistsAsync())
                        await blobClient.UploadAsync(audioStream, blobHttpHeaders);
                    
                    var audioSasLink = await CreateServiceSASBlob(blobClient);

                    keyword.AudioLink = audioSasLink.ToString();
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    throw;
                }

                _keywordEntityRepository.Update(keyword, "system");
                _keywordEntityRepository.SaveAndStopTracking();
            };

            synthesizer.SynthesisCanceled += (o, e) =>
            {
                SpeechSynthesisCancellationDetails cancellation =
                    SpeechSynthesisCancellationDetails.FromResult(e.Result);

                throw new Exception(cancellation.ErrorDetails);
            };
        }

        var updatedKeyword = _keywordEntityRepository.GetById(keywordId);
        return _mapper.Map<Keyword>(updatedKeyword);
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