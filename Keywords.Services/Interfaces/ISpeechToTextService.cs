using Keywords.API.Swagger.Controllers.Generated;
using Microsoft.AspNetCore.Http;

namespace Keywords.Services.Interfaces;

public interface ISpeechToTextService
{
    Task<PronunciationAssessmentResponseDTO> CreatePronunciationAssessment(string language, string referenceText, IFormFile file);
}