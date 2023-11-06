using Keywords.API.Swagger.Controllers.Generated;
using Keywords.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Keywords.API.Controllers;

[ApiController]
[Route("[controller]")]
public class IndexerController : ControllerBase
{
    private readonly IIndexerService _indexerService;
    private readonly ILogger<IndexerController> _logger;

    public IndexerController(IIndexerService indexerService, ILogger<IndexerController> logger)
    {
        _indexerService = indexerService;
        _logger = logger;
    }

    [HttpGet("GetIndexerOutput")]
    public Task<ICollection<Video>> GetIndexerOutput(string videoId)
    {
        return _indexerService.GetIndexerOutputAsync(videoId);
    }
    
    [HttpPost( "IndexVideo")]
    public Task<RequestVideoIndexResponse> IndexVideo(string url, string videoName, string description)
    {
        return _indexerService.IndexVideoAsync(url, videoName, description);
    }
}