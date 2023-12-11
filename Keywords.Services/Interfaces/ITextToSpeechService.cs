using Keywords.API.Swagger.Controllers.Generated;

namespace Keywords.Services.Interfaces;

public interface ITextToSpeechService
{
    /// <summary>
    /// Creates an audio file and attaches its URL to the entity
    /// </summary>
    /// <param name="keywordId">Id of the keyword</param>
    /// <returns>Returns the keyword with the audioUrl attached</returns>
    Task<Keyword?> CreateAudio(Guid keywordId);
}