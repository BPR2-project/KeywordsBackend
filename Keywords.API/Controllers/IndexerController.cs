using Keywords.API.Swagger.Controllers.Generated;
using Keywords.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Keywords.API.Controllers;

public class IndexerController : IndexerControllerBase
{
    private readonly IIndexerService _indexerService;

    public IndexerController(IIndexerService indexerService)
    {
        _indexerService = indexerService;
    }
    
    public override async Task<ActionResult<IndexerProgress>> GetIndexerProgress(Guid videoId)
    {
        var indexerOutput = await _indexerService.GetIndexerProgressAsync(videoId);
        if (indexerOutput == null)
        {
            return NotFound();
        }

        return indexerOutput;
    }

    public override async Task<IActionResult> IndexVideo(Guid videoId, string url)
    {
        await _indexerService.IndexVideoAsync(videoId, url);
        return Ok();
    }
}