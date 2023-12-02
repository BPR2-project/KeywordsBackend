using AutoMapper;
using Keywords.API.Swagger.Controllers.Generated;

namespace Keywords.Mappers;

public class AzureSpeechToTextMapper : Profile
{
    public AzureSpeechToTextMapper()
    {
        CreateMap<speechToText_api.FileParameter, FileParameter>();
        CreateMap<speechToText_api.PronunciationAssessment, PronunciationAssessmentDTO>();
        CreateMap<speechToText_api.PronunciationAssessmentResponse, PronunciationAssessmentResponseDTO>();
    }
}