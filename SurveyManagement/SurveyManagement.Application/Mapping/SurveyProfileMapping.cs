using AutoMapper;
using SurveyManagement.Domain.Entities;
using SurveyManagement.Application.DTOs.SurveyDTOs;

namespace SurveyManagement.Application.Mapping
{
    public class SurveyProfileMapping : Profile
    {
        public SurveyProfileMapping()
        {
            CreateMap<Survey, SurveyDto>().ReverseMap();

            CreateMap<CreateSurveyDto, Survey>()
                .ForMember(dest => dest.SurveyId, opt => opt.MapFrom(_ => Guid.NewGuid()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.CreatedByUserId, opt => opt.Ignore()); // Important!
        }
    }
}
