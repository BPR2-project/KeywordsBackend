using Keywords.API.Swagger.Controllers.Generated;

namespace Keywords.Services.Interfaces;

public interface IIndexerService
{
    /// <summary>
    /// Gets the indexing status for the video
    /// </summary>
    /// <param name="videoId">Id of the video</param>
    /// <returns>Returns the indexing status for teh video</returns>
    Task<IndexerProgress?> GetIndexerProgressAsync(Guid videoId);
    /// <summary>
    /// Starts the process of indexing a video
    /// </summary>
    /// <param name="videoId">Id of the video to ve indexed</param>
    /// <param name="url">Url of the video to be indexed</param>
    Task IndexVideoAsync(Guid videoId, string url);
}