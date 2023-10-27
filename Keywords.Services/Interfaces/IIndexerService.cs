using indexer_api;

namespace Keywords.Services.Interfaces;

public interface IIndexerService
{
    Task<ICollection<Ocr>> GetKeywordsAsync(string videoId);
}