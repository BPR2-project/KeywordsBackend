using AutoMapper;
using Keywords.API.Swagger.Controllers.Generated;
using Keywords.Data.Repositories.Interfaces;
using Keywords.Services.Interfaces;

namespace Keywords.Services;

public class KeywordService: IKeywordService
{
    private IKeywordEntityRepository _keywordEntityRepository;
    private IMapper _mapper;

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

    public bool ExistsById(Guid id)
    {
        return _keywordEntityRepository.ExistsById(id);
    }
}