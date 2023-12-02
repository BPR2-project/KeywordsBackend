using AutoMapper;
using indexer_api;
using Keywords.API.Swagger.Controllers.Generated;
using Keywords.Data;

namespace Keywords.Mappers;

public class IndexerMapper: Profile
{
    public IndexerMapper()
    {
        CreateMap<Video, IndexerResponse>()
            .ForMember(a => a.State,
                opt => opt.MapFrom(a => IndexerState.Indexing));
        CreateMap<IEnumerable<KeywordEntity>, IndexerResponse>()
            .ForMember(a => a.State,
                opt => opt.MapFrom(a => IndexerState.Succeeded))
            .ForMember(a => a.Keywords,
            opt => opt.MapFrom(a => a));
        CreateMap<IndexerEntity, IndexerResponse>();
        CreateMap<KeywordEntity, Keyword>();
    }
}