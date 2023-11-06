using Keywords.API.Swagger.Controllers.Generated;

namespace Keywords.Services.Interfaces;

public interface IKeywordService
{
    Keyword? GetKeyword(Guid id);
    (List<Keyword> keywords, int totalSize) GetAllKeywordsByVideoId(Guid videoId, int size, int page);
    Keyword? PublishKeyword(Guid id, bool toBePublished);
    bool KeywordsVideoExistsById(Guid videoId);
}