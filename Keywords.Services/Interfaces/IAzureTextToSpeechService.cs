using Keywords.API.Swagger.Controllers.Generated;

namespace Keywords.Services.Interfaces;

public interface IAzureTextToSpeechService
{
    Task<Keyword> CreateAudio(Guid keywordId);
}