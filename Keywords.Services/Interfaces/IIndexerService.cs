using Keywords.API.Swagger.Controllers.Generated;

namespace Keywords.Services.Interfaces;

public interface IIndexerService
{
    Task<ICollection<Ocr>> GetKeywordsAsync(string videoId);
}