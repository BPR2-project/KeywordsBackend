using AutoMapper;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using Keywords.API.Swagger.Controllers.Generated;
using Keywords.Data.Repositories.Interfaces;
using Keywords.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using textToSpeech_api;
using Microsoft.CognitiveServices.Speech;

namespace Keywords.Services;

public class TextToSpeechService : ITextToSpeechService
{
    private readonly IKeywordEntityRepository _keywordEntityRepository;
    private readonly IAzureTextToSpeechClient _azureTextToSpeechClient;
    private readonly IMapper _mapper;
    private readonly string _ttsSubscriptionKey;
    private readonly string _ttsRegion;
    private readonly string _blobUri;
    private readonly string _blobContainer;

    public TextToSpeechService(IAzureTextToSpeechClient azureTextToSpeechClient,
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
    
    public async Task<Keyword?> CreateAudio(Guid keywordId)
    {
        var voicesList = await _azureTextToSpeechClient.GetAllVoicesAsync(_ttsSubscriptionKey);

        var exists = _keywordEntityRepository.ExistsById(keywordId);

        if (!exists)
            return null;

        var keywordEntity = _keywordEntityRepository.GetById(keywordId);

        if (!string.IsNullOrEmpty(keywordEntity.AudioLink))
            return _mapper.Map<Keyword>(keywordEntity);
        
        var keywordLanguage = keywordEntity.Language;

        var speechConfig = SpeechConfig.FromSubscription(_ttsSubscriptionKey, _ttsRegion);
        speechConfig.SpeechSynthesisVoiceName = voicesList
            .First(a => a.Locale == keywordLanguage && a.Gender == "Female").ShortName;

        var containerClient = new BlobContainerClient(_blobUri, _blobContainer);

        using (var synthesizer = new SpeechSynthesizer(speechConfig, null))
        {
            var synthesisCompleted = false;
            
            synthesizer.SynthesisCompleted += async (o, e) =>
            {
                var audioBuffer = e.Result.AudioData;

                var audioStream = new MemoryStream(audioBuffer);
                var blobClient = containerClient.GetBlobClient($"{keywordEntity.Content}.wav");

                BlobHttpHeaders blobHttpHeaders = new BlobHttpHeaders()
                {
                    ContentType = "audio/wav"
                };

                try 
                {
                    if (!await blobClient.ExistsAsync())
                        await blobClient.UploadAsync(audioStream, blobHttpHeaders);
                    
                    var audioSasLink = await CreateServiceSasBlob(blobClient);

                    keywordEntity.AudioLink = audioSasLink.ToString();
                    
                    _keywordEntityRepository.Update(keywordEntity, "system");
                    _keywordEntityRepository.SaveAndStopTracking();

                    synthesisCompleted = true;
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    throw;
                }
            };
            
            synthesizer.SynthesisCanceled += (o, e) =>
            {
                var cancellation = SpeechSynthesisCancellationDetails.FromResult(e.Result);

                throw new Exception(cancellation.ErrorDetails);
            };
            
            await synthesizer.SpeakTextAsync(keywordEntity.Content);

            while (!synthesisCompleted)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }

        var updatedKeyword = _keywordEntityRepository.GetById(keywordId);
        return _mapper.Map<Keyword>(updatedKeyword);
    }

    private async Task<Uri> CreateServiceSasBlob(
        BlobClient blobClient,
        string? storedPolicyName = null)
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

            var sasUri = blobClient.GenerateSasUri(sasBuilder);

            return sasUri;
        }
        else
        {
            // Client object is not authorized via Shared Key
            return null;
        }
    }
}