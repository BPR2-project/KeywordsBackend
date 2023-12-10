using Keywords.API.Swagger.Controllers.Generated;
using Keywords.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Keywords.API.Controllers;

public class TextToSpeechController : TextToSpeechControllerBase
{
    private readonly ITextToSpeechService _textToSpeechService;
    private readonly IKeywordService _keywordService;

    public TextToSpeechController(ITextToSpeechService textToSpeechService, IKeywordService keywordService)
    {
        _textToSpeechService = textToSpeechService;
        _keywordService = keywordService;
    }

    public override Task<ActionResult<Keyword>> CreateAudio(Guid id)
    {
        return Task.Run<ActionResult<Keyword>>(async () =>
        {
            var keyword = _keywordService.GetKeyword(id);
            if (keyword == null)
                return NotFound();
            
            var updatedKeyword = await _textToSpeechService.CreateAudio(id);
            return Ok(updatedKeyword);
        });
    }
}