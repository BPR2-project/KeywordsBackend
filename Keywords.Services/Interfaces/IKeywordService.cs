using Keywords.API.Swagger.Controllers.Generated;

namespace Keywords.Services.Interfaces;

public interface IKeywordService
{
    Keyword? GetKeyword(Guid id);
}