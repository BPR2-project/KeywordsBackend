using Keywords.API.Swagger.Controllers.Generated;

namespace Keywords.Services.Interfaces;

public interface IIndexerService
{
    Task<VideoIndexerResponse> GetIndexerOutputAsync(Guid videoId);
    Task IndexVideoAsync(Guid videoId, string url);
}