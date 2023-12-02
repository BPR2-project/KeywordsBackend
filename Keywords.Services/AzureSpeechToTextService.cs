using System.Text;
using AutoMapper;
using Keywords.API.Swagger.Controllers.Generated;
using Keywords.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using speechToText_api;
namespace Keywords.Services;

public class AzureSpeechToTextService : IAzureSpeechToTextService
{
    private readonly IAzureSpeechToTextClient _azureSpeechToTextClient;
    private IMapper _mapper;
    private string _sttSubscriptionKey;
    
    public AzureSpeechToTextService(IAzureSpeechToTextClient azureSpeechToTextClient, IMapper mapper, 
        IConfiguration configuration)
    {
        _azureSpeechToTextClient = azureSpeechToTextClient;
        _mapper = mapper;
        _sttSubscriptionKey = configuration["SpeechToText:Key"];
    }

    public async Task<PronunciationAssessmentResponseDTO> CreatePronunciationAssessment(string language, string referenceText, IFormFile file)
    {
        var pronAssessmentParamsJson = $"{{\"ReferenceText\":\"{referenceText}\",\"GradingSystem\":\"HundredMark\",\"Granularity\":\"FullText\",\"Dimension\":\"Comprehensive\"}}";
        var pronAssessmentParamsBytes = Encoding.UTF8.GetBytes(pronAssessmentParamsJson);
        var pronAssessmentParams = Convert.ToBase64String(pronAssessmentParamsBytes);
        using(var ms = new MemoryStream()) {
            await file.CopyToAsync(ms);
            ms.Seek(0, SeekOrigin.Begin);
        var pronunciationAssessment = await _azureSpeechToTextClient.CreatePronunciationAssessmentAsync(language, 
            _sttSubscriptionKey, pronAssessmentParams, ms);
        var pronunciationAssessmentResponse =
            _mapper.Map<PronunciationAssessmentResponseDTO>(pronunciationAssessment);
        return pronunciationAssessmentResponse;
        }
    }
}