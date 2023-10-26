namespace Keywords.Data.Repositories.Interfaces;

public interface IKeywordEntityRepository: IRepository<KeywordEntity>
{
    bool ExistsById(Guid id);
}