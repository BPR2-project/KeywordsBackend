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

    public override Task<ActionResult<PronunciationAssessmentResponseDTO>> CreatePronunciationAssessment(string language, 
        string referenceText, FileParameter file)
    {
        return Task.Run<ActionResult<PronunciationAssessmentResponseDTO>>(async () =>
        {
            PronunciationAssessmentResponseDTO response = await _azureSpeechToTextService.CreatePronunciationAssessment(
                language, referenceText, file);
            return Ok(response);
        });
    }
}