using Keywords.API.Swagger.Controllers.Generated;
using Keywords.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Keywords.API.Controllers;

public class AzureSpeechToTextController: SpeechControllerBase
{
    private readonly IAzureSpeechToTextService _azureSpeechToTextService;
    
    public AzureSpeechToTextController(IAzureSpeechToTextService azureSpeechToTextService)
    {
        _azureSpeechToTextService = azureSpeechToTextService;
    }

    public override Task<ActionResult<PronunciationAssessmentResponseDTO>> CreatePronunciationAssessment(IFormFile upfile, string language, string referenceText)
    {
        return Task.Run<ActionResult<PronunciationAssessmentResponseDTO>>(async () =>
        {
            PronunciationAssessmentResponseDTO response = await _azureSpeechToTextService.CreatePronunciationAssessment(
                language, referenceText, upfile);
            return Ok(response);
        });
    }
}