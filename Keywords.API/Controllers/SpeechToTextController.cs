using Keywords.API.Swagger.Controllers.Generated;
using Keywords.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Keywords.API.Controllers;

public class SpeechToTextController: SpeechControllerBase
{
    private readonly ISpeechToTextService _speechToTextService;
    
    public SpeechToTextController(ISpeechToTextService speechToTextService)
    {
        _speechToTextService = speechToTextService;
    }

    public override Task<ActionResult<PronunciationAssessmentResponseDTO>> CreatePronunciationAssessment(IFormFile body, string language, string referenceText)
    {
        return Task.Run<ActionResult<PronunciationAssessmentResponseDTO>>(async () =>
        {
            PronunciationAssessmentResponseDTO response = await _speechToTextService.CreatePronunciationAssessment(
                language, referenceText, body);
            return Ok(response);
        });
    }
}