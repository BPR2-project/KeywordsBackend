using Keywords.API.Swagger.Controllers.Generated;

namespace Keywords.Services.Interfaces;

public interface IAzureSpeechToTextService
{
    Task<PronunciationAssessmentResponseDTO> CreatePronunciationAssessment(string language, string referenceText, FileParameter file);
}