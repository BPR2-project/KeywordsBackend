using Keywords.API.Swagger.Controllers.Generated;
using Keywords.Extensions;
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

    public override Task<ActionResult<PaginatedKeywordsResponse>> GetAllKeywordsByVideoId(PaginatedKeywordsRequest paginatedKeywordsRequest)
    {
        return Task.Run<ActionResult<PaginatedKeywordsResponse>>(() =>
        {
            var modelState = ModelState.GetAllErrors();
            if (modelState.Any())
                return BadRequest(string.Join("\n", modelState));

            var videoExists = _keywordService.KeywordsVideoExistsById(paginatedKeywordsRequest.VideoId.Value);

            if (!videoExists)
                return NotFound();
            
            if (paginatedKeywordsRequest.Size is 0 or null)
                paginatedKeywordsRequest.Size = 10;
            if (paginatedKeywordsRequest.Page is null)
                paginatedKeywordsRequest.Page = 0;

            var allVideos = _keywordService.GetAllKeywordsByVideoId(paginatedKeywordsRequest.VideoId.Value, paginatedKeywordsRequest.Size.Value, paginatedKeywordsRequest.Page.Value);

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
                TotalAmountOfPages = (int)Math.Ceiling((double)allVideos.totalSize / paginatedKeywordsRequest.Size.Value)
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