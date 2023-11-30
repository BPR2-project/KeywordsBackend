using System.Text;
using AutoMapper;
using Keywords.API.Swagger.Controllers.Generated;
using Keywords.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using speechToText_api;
using FileParameter = speechToText_api.FileParameter;
using FileParameterDTO = Keywords.API.Swagger.Controllers.Generated.FileParameter;
namespace Keywords.Services;

public class AzureSpeechToTextService : IAzureSpeechToTextService
{
    private readonly IAzureSpeechToTextClient _azureSpeechToTextClient;
    private IMapper _mapper;
    private string _sttSubscriptionKey;
    private string _sttRegion;
    private string _contentType;

    public AzureSpeechToTextService(IAzureSpeechToTextClient azureSpeechToTextClient, IMapper mapper, 
        IConfiguration configuration)
    {
        _azureSpeechToTextClient = azureSpeechToTextClient;
        _mapper = mapper;
        _sttSubscriptionKey = "f80fb1904ffc49318537131d2412dd2a"; //configuration["SpechToText:Key"]; // Needs to be set
        _sttRegion = configuration["Azure:Region"];
        _contentType = $"audio/wav; codecs=audio/pcm; samplerate=16000";
    }

    public async Task<PronunciationAssessmentResponseDTO> CreatePronunciationAssessment(string language, string referenceText, FileParameterDTO file)
    {
        var fileChunk = _mapper.Map<FileParameter>(file);
        var pronAssessmentParamsJson = $"{{\"ReferenceText\":\"{referenceText}\",\"GradingSystem\":\"HundredMark\",\"Granularity\":\"FullText\",\"Dimension\":\"Comprehensive\"}}";
        var pronAssessmentParamsBytes = Encoding.UTF8.GetBytes(pronAssessmentParamsJson);
        var pronAssessmentParams = Convert.ToBase64String(pronAssessmentParamsBytes);
        var pronunciationAssessment = await _azureSpeechToTextClient.CreatePronunciationAssessmentAsync(language, 
            fileChunk, _sttSubscriptionKey, _contentType, pronAssessmentParams, 
            "application/json;text/xml", false);
        var pronunciationAssessmentResponse =
            _mapper.Map<PronunciationAssessmentResponseDTO>(pronunciationAssessment);
        return pronunciationAssessmentResponse;
    }
}