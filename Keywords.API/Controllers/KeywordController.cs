using Keywords.API.Swagger.Controllers.Generated;
using Keywords.Extensions;
using Keywords.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Keywords.API.Controllers;

public class KeywordController: KeywordControllerBase
{
    private readonly IKeywordService _keywordService;
    private readonly ITextToSpeechService _textToSpeechService;

    public KeywordController(IKeywordService keywordService, ITextToSpeechService textToSpeechService)
    {
        _keywordService = keywordService;
        _textToSpeechService = textToSpeechService;
    }

    public override Task<ActionResult<PaginatedKeywordsResponse>> GetAllKeywordsByVideoId(PaginatedKeywordsRequest paginatedKeywordsRequest)
    {
        return Task.Run<ActionResult<PaginatedKeywordsResponse>>(() =>
        {
            var modelState = ModelState.GetAllErrors();
            if (modelState.Any())
                return BadRequest(string.Join("\n", modelState));
            
            var videoExists = _keywordService.KeywordsVideoExistsById(paginatedKeywordsRequest.VideoId);
            
            if (!videoExists)
                return NotFound();
            
            if (paginatedKeywordsRequest.Size is 0)
                paginatedKeywordsRequest.Size = 10;

            var allVideos = _keywordService.GetAllKeywordsByVideoId(
                paginatedKeywordsRequest.VideoId, paginatedKeywordsRequest.Size, paginatedKeywordsRequest.Page, paginatedKeywordsRequest.Published);

            if (allVideos.keywords.Any() == false)
                return Ok(new PaginatedKeywordsResponse()
                {
                    Keywords = new List<Keyword>(),
                    CurrentPage = 1,
                    SizeRequested = paginatedKeywordsRequest.Size,
                    TotalAmount = 0,
                    TotalAmountOfPages = 1
                });
            var response = new PaginatedKeywordsResponse()
            {
                Keywords = allVideos.keywords,
                CurrentPage = paginatedKeywordsRequest.Page,
                SizeRequested = paginatedKeywordsRequest.Size,
                TotalAmount = allVideos.totalSize,
                TotalAmountOfPages = (int)Math.Ceiling((double)allVideos.totalSize / paginatedKeywordsRequest.Size)
            };
            return Ok(response);
        });
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

    public override Task<ActionResult<Keyword>> PublishKeyword(Guid keywordId, bool? toBePublished)
    {
        return Task.Run<ActionResult<Keyword>>(() =>
        {
            var keyword = _keywordService.PublishKeyword(keywordId, toBePublished ?? false);

            if (keyword == null)
                return NotFound();

            return Ok(keyword);
        });
    }
}