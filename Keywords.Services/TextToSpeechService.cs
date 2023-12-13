using AutoMapper;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using Keywords.API.Swagger.Controllers.Generated;
using Keywords.Data.Repositories.Interfaces;
using Keywords.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.CognitiveServices.Speech;

namespace Keywords.Services;

public class TextToSpeechService : ITextToSpeechService
{
    private readonly IKeywordEntityRepository _keywordEntityRepository;
    private readonly IMapper _mapper;
    private readonly string _ttsSubscriptionKey;
    private readonly string _ttsRegion;
    private readonly string _blobUri;
    private readonly string _blobContainer;

    public TextToSpeechService(IKeywordEntityRepository keywordEntityRepository, IConfiguration configuration,
        IMapper mapper)
    {
        _blobUri = configuration["BlobStorage:Uri"];
        _blobContainer = configuration["BlobStorage:Container"];
        _ttsSubscriptionKey = configuration["TextToSpeech:Key"];
        _ttsRegion = configuration["Azure:Region"];
        _keywordEntityRepository = keywordEntityRepository;
        _mapper = mapper;
    }

    public async Task<Keyword?> CreateAudio(Guid keywordId)
    {
        SpeechConfig speechConfig;
        
        try
        {
            speechConfig = SpeechConfig.FromSubscription(_ttsSubscriptionKey, _ttsRegion);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
        
        var keywordEntity = _keywordEntityRepository.GetById(keywordId);

        if (!string.IsNullOrEmpty(keywordEntity.AudioLink))
            return _mapper.Map<Keyword>(keywordEntity);

        using (var voicesFetcher = new SpeechSynthesizer(speechConfig, null))
        {
            var keywordLanguage = keywordEntity.Language;
            var voicesList = await voicesFetcher.GetVoicesAsync(keywordLanguage);
            var selectedVoice = voicesList.Voices.First(a => a.Gender == SynthesisVoiceGender.Female).ShortName;
            speechConfig.SpeechSynthesisVoiceName = selectedVoice;
        }
        
        using (var synthesizer = new SpeechSynthesizer(speechConfig, null))
        {
            var synthesisCompleted = false;

            synthesizer.SynthesisCompleted += async (_, e) =>
            {
                var audioBuffer = e.Result.AudioData;
                var containerClient = new BlobContainerClient(_blobUri, _blobContainer);
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
                catch(Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            };

            synthesizer.SynthesisCanceled += (_, e) =>
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