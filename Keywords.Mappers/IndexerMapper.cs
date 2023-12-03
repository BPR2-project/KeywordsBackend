using AutoMapper;
using indexer_api;
using Keywords.API.Swagger.Controllers.Generated;
using Keywords.Data;

namespace Keywords.Mappers;

public class IndexerMapper: Profile
{
    public IndexerMapper()
    {
        CreateMap<Video, IndexerProgress>()
            .ForMember(a => a.State,
                opt => opt.MapFrom(a => IndexerState.Indexing));
        CreateMap<IEnumerable<KeywordEntity>, IndexerProgress>()
            .ForMember(a => a.State,
                opt => opt.MapFrom(a => IndexerState.Succeeded));
        CreateMap<IndexerEntity, IndexerProgress>();
        CreateMap<KeywordEntity, Keyword>();
    }
}