using AutoMapper;
using Keywords.API.Swagger.Controllers.Generated;

namespace Keywords.Mappers;

public class SpeechToTextMapper : Profile
{
    public SpeechToTextMapper()
    {
        CreateMap<speechToText_api.FileParameter, FileParameter>();
        CreateMap<speechToText_api.PronunciationAssessment, PronunciationAssessmentDTO>();
        CreateMap<speechToText_api.PronunciationAssessmentResponse, PronunciationAssessmentResponseDTO>();
    }
}