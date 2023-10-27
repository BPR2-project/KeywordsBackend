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

    [HttpGet(Name = "GetIndexerOutput")]
    public Task<ICollection<Ocr>> GetIndexerOutput(string videoId)
    {
        return _indexerService.GetKeywordsAsync(videoId);
    }
}