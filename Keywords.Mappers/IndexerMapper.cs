using AutoMapper;
using Keywords.API.Swagger.Controllers.Generated;

namespace Keywords.Mappers;

public class IndexerMapper: Profile
{
    public IndexerMapper()
    {
        CreateMap<indexer_api.Video, Video>();
        CreateMap<indexer_api.IndexVideoReceipt, RequestVideoIndexResponse>()
            .ForMember(a => a.Receipt, opt => opt.MapFrom(a => a));
        CreateMap<indexer_api.IndexInProgress, RequestVideoIndexResponse>()
            .ForMember(a => a.IndexInProgress, opt => opt.MapFrom(a => a));
        CreateMap<indexer_api.IndexVideoReceipt, IndexVideoReceipt>();
        CreateMap<indexer_api.IndexInProgress, IndexInProgress>();
        CreateMap<indexer_api.Insights, Insights>();
        CreateMap<indexer_api.Ocr, Ocr>();
    }
}