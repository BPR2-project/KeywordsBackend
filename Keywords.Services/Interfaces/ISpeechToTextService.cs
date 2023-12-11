using Keywords.API.Swagger.Controllers.Generated;
using Microsoft.AspNetCore.Http;

namespace Keywords.Services.Interfaces;

public interface ISpeechToTextService
{
    /// <summary>
    /// Creates a pronunciation assessment request
    /// </summary>
    /// <param name="language">Language of the keyword</param>
    /// <param name="referenceText">Keyword to be used for assessment</param>
    /// <param name="file">Voice recording to assess the pronunciation for</param>
    /// <returns>Returns the pronunciation assessment</returns>
    Task<PronunciationAssessmentResponseDTO> CreatePronunciationAssessment(string language, string referenceText, IFormFile file);
}