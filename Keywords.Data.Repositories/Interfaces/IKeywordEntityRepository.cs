namespace Keywords.Data.Repositories.Interfaces;

public interface IKeywordEntityRepository: IRepository<KeywordEntity>
{
    bool ExistsById(Guid id);
    bool KeywordsVideoExistsById(Guid videoId);
    (List<KeywordEntity>? keywords, int totalsize) GetAllKeywordsByVideoId(Guid videoId, int size, int page, bool? published = false);
}