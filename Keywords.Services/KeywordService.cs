using AutoMapper;
using Keywords.API.Swagger.Controllers.Generated;
using Keywords.Data.Repositories.Interfaces;
using Keywords.Services.Interfaces;

namespace Keywords.Services;

public class KeywordService : IKeywordService
{
    private readonly IKeywordEntityRepository _keywordEntityRepository;
    private readonly IMapper _mapper;

    public KeywordService(IKeywordEntityRepository keywordEntityRepository, IMapper mapper)
    {
        _keywordEntityRepository = keywordEntityRepository;
        _mapper = mapper;
    }

    public Keyword? GetKeyword(Guid id)
    {
        if (!ExistsById(id))
            return null;

        var keywordEntity = _keywordEntityRepository.GetById(id);

        return _mapper.Map<Keyword>(keywordEntity);
    }

    public (List<Keyword> keywords, int totalSize) GetAllKeywordsByVideoId(Guid videoId, int size, int page,
        bool published)
    {
        var keywordsEntities = _keywordEntityRepository.GetAllKeywordsByVideoId(videoId, size, page, published);

        if (keywordsEntities.keywords == null)
            return (new List<Keyword>(), 0);
        
        var keywords = keywordsEntities.keywords
            .Select(a => _mapper.Map<Keyword>(a))
            .ToList();

        return (keywords, keywordsEntities.totalsize);
    }

    public Keyword? PublishKeyword(Guid id, bool toBePublished)
    {
        if (!ExistsById(id))
            return null;

        var keyword = _keywordEntityRepository.GetById(id);
        keyword.IsPublished = toBePublished;

        _keywordEntityRepository.Update(keyword, "system");
        _keywordEntityRepository.Save();

        return _mapper.Map<Keyword>(keyword);
    }

    private bool ExistsById(Guid id)
    {
        return _keywordEntityRepository.ExistsById(id);
    }

    public bool KeywordsVideoExistsById(Guid videoId)
    {
        return _keywordEntityRepository.KeywordsVideoExistsById(videoId);
    }
}