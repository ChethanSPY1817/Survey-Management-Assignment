using AutoMapper;
using SurveyManagement.Domain.Entities;
using SurveyManagement.Application.DTOs.ResponseDTOs;

namespace SurveyManagement.Application.Mapping
{
    public class ResponseProfileMapping : Profile
    {
        public ResponseProfileMapping()
        {
            CreateMap<Response, ResponseDto>().ReverseMap();
            CreateMap<CreateResponseDto, Response>()
                .ForMember(dest => dest.ResponseId, opt => opt.Ignore())
                .ForMember(dest => dest.AnsweredAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
        }
    }
}
