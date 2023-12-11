using Keywords.API.Swagger.Controllers.Generated;

namespace Keywords.Services.Interfaces;

public interface IKeywordService
{
    /// <summary>
    /// Get a keyword by id
    /// </summary>
    /// <param name="id">Id of the keyword</param>
    /// <returns>Returns a keyword object</returns>
    Keyword? GetKeyword(Guid id);
    
    /// <summary>
    /// Gets a paginated and filtered list of keywords by video id
    /// </summary>
    /// <param name="videoId">Id of the video to retrieve the keywords for</param>
    /// <param name="size">Size of the page</param>
    /// <param name="page">Page number</param>
    /// <param name="published">Flag for keywords filter</param>
    /// <returns>Returns a paginated and filtered list of keywords</returns>
    (List<Keyword> keywords, int totalSize) GetAllKeywordsByVideoId(Guid videoId, int size, int page, bool published);
    
    /// <summary>
    /// Action to publish or unpublish a keyword
    /// </summary>
    /// <param name="id">Id of the keyword</param>
    /// <param name="toBePublished">State of the published flag</param>
    /// <returns>Returns the keyword with updated flag</returns>
    Keyword? PublishKeyword(Guid id, bool toBePublished);
    
    /// <summary>
    /// Checks if the video with the supplied id belongs to a keyword
    /// </summary>
    /// <param name="videoId">Id of the video</param>
    /// <returns>Returns true if there are keywords that belong to the video with the specified id</returns>
    bool KeywordsVideoExistsById(Guid videoId);
}