using Keywords.API.Swagger.Controllers.Generated;
using Keywords.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Keywords.API.Controllers;

public class IndexerController : IndexerControllerBase
{
    private readonly IIndexerService _indexerService;
    private readonly ILogger<IndexerController> _logger;

    public IndexerController(IIndexerService indexerService, ILogger<IndexerController> logger)
    {
        _indexerService = indexerService;
        _logger = logger;
    }
    
    public override async Task<ActionResult<IndexerResponse>> GetIndexerResponse(Guid videoId)
    {
        var indexerOutput = await _indexerService.GetIndexerOutputAsync(videoId);
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