using AutoMapper;
using Keywords.API.Swagger.Controllers.Generated;
using Keywords.Data;

namespace Keywords.Mappers;

public class KeywordMapper: Profile
{
    public KeywordMapper()
    {
        CreateMap<KeywordEntity, Keyword>()
            .ForMember(a => a.Id, opt => opt.MapFrom(a => a.Id))
            .ForMember(a => a.Content, opt => opt.MapFrom(a => a.Content))
            .ForMember(a => a.IsPublished, opt => opt.MapFrom(a => a.IsPublished))
            .ForMember(a => a.VideoId, opt => opt.MapFrom(a => a.VideoId))
            .ForMember(a => a.Language, opt => opt.MapFrom(a => a.Language))
            .ForMember(a => a.AudioLink, opt => opt.MapFrom(a => a.AudioLink));
        
        CreateMap<Keyword, KeywordEntity>()
            .ForMember(a => a.Id, opt => opt.MapFrom(a => a.Id))
            .ForMember(a => a.Content, opt => opt.MapFrom(a => a.Content))
            .ForMember(a => a.IsPublished, opt => opt.MapFrom(a => a.IsPublished))
            .ForMember(a => a.VideoId, opt => opt.MapFrom(a => a.VideoId))
            .ForMember(a => a.Language, opt => opt.MapFrom(a => a.Language))
            .ForMember(a => a.AudioLink, opt => opt.MapFrom(a => a.AudioLink));
    }
}