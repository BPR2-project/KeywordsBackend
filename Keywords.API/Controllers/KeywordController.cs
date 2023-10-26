using Keywords.API.Swagger.Controllers.Generated;
using Keywords.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Keywords.API.Controllers;

public class KeywordController: KeywordControllerBase
{
    private readonly IKeywordService _keywordService;

    public KeywordController(IKeywordService keywordService)
    {
        _keywordService = keywordService;
    }

    public override Task<ActionResult<Keyword>> GetKeyword(Guid keywordId)
    {
        return Task.Run<ActionResult<Keyword>>(() =>
        {
            var keyword = _keywordService.GetKeyword(keywordId);

            if (keyword == null)
                return NotFound();
            
            return Ok(keyword);
        });
    }
}