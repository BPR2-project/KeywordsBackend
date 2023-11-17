using Keywords.API.Swagger.Controllers.Generated;

namespace Keywords.Services.Interfaces;

public interface IIndexerService
{
    Task<IndexerResponse?> GetIndexerOutputAsync(Guid videoId);
    Task IndexVideoAsync(Guid videoId, string url);
}