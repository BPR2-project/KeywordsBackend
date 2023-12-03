using Keywords.API.Swagger.Controllers.Generated;

namespace Keywords.Services.Interfaces;

public interface IIndexerService
{
    Task<IndexerProgress?> GetIndexerProgressAsync(Guid videoId);
    Task IndexVideoAsync(Guid videoId, string url);
}