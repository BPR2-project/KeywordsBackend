using Keywords.API.Swagger.Controllers.Generated;

namespace Keywords.Services.Interfaces;

public interface ITextToSpeechService
{
    Task<Keyword?> CreateAudio(Guid keywordId);
}