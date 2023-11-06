using Keywords.API.Swagger.Controllers.Generated;

namespace Keywords.Services.Interfaces;

public interface IIndexerService
{
    Task<ICollection<Video>> GetIndexerOutputAsync(string videoId);
    Task<RequestVideoIndexResponse> IndexVideoAsync(string url, string videoName, string description);
}