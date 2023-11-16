using Keywords.API.Swagger.Controllers.Generated;
using Keywords.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Keywords.API.Controllers;

[ApiController]
[Route("[controller]")]
public class IndexerController : IndexerControllerBase
{
    private readonly IIndexerService _indexerService;
    private readonly ILogger<IndexerController> _logger;

    public IndexerController(IIndexerService indexerService, ILogger<IndexerController> logger)
    {
        _indexerService = indexerService;
        _logger = logger;
    }
    
    public override Task<ActionResult<VideoIndexerResponse>> GetKeywordsFromIndexer(Guid videoId)
    {
        throw new NotImplementedException();
    }

    public override async Task<IActionResult> IndexVideo(Guid videoId, string url)
    {
        await _indexerService.IndexVideoAsync(videoId, url);
        return Ok();
    }
}