using Keywords.API.Swagger.Controllers.Generated;
using Keywords.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Keywords.API.Controllers;

public class AzureTextToSpeechController : TextToSpeechControllerBase
{
    private readonly IAzureTextToSpeechService _azureTextToSpeechService;
    private readonly IKeywordService _keywordService;

    public AzureTextToSpeechController(IAzureTextToSpeechService azureTextToSpeechService, IKeywordService keywordService)
    {
        _azureTextToSpeechService = azureTextToSpeechService;
        _keywordService = keywordService;
    }

    public override Task<ActionResult<Keyword>> CreateAudio(Guid id)
    {
        return Task.Run<ActionResult<Keyword>>(async () =>
        {
            var keyword = _keywordService.GetKeyword(id);
            if (keyword == null)
                return NotFound();
            
            var updatedKeyword = await _azureTextToSpeechService.CreateAudio(id);
            return Ok(updatedKeyword);
        });
    }
}