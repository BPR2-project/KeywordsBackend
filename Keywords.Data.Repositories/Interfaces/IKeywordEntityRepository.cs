namespace Keywords.Data.Repositories.Interfaces;

public interface IKeywordEntityRepository: IRepository<KeywordEntity>
{
    /// <summary>
    /// Checks if the keyword entity with the id exists in db
    /// </summary>
    /// <param name="id">Id of the keyword</param>
    /// <returns>True if it exists</returns>
    bool ExistsById(Guid id);
    
    /// <summary>
    /// Checks if the video with the supplied id belongs to a keyword entity
    /// </summary>
    /// <param name="videoId">Id of the video</param>
    /// <returns>Returns true if there are keywords that belong to the video with the specified id</returns>
    bool KeywordsVideoExistsById(Guid videoId);
    
    /// <summary>
    /// Gets a paginated and filtered list of keywords by video id
    /// </summary>
    /// <param name="videoId">Id of the video to retrieve the keywords for</param>
    /// <param name="size">Size of the page</param>
    /// <param name="page">Page number</param>
    /// <param name="published">Flag for keywords filter</param>
    /// <returns>Returns a paginated and filtered list of keywords together with the total size in the db</returns>
    (List<KeywordEntity>? keywords, int totalsize) GetAllKeywordsByVideoId(Guid videoId, int size, int page, bool? published = false);
}