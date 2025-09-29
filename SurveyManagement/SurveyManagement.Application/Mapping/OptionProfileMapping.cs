using AutoMapper;
using SurveyManagement.Domain.Entities;
using SurveyManagement.Application.DTOs.OptionDTOs;

namespace SurveyManagement.Application.Mapping
{
    public class OptionProfileMapping : Profile
    {
        public OptionProfileMapping()
        {
            CreateMap<Option, OptionDto>().ReverseMap();
            CreateMap<CreateOptionDto, Option>();
            CreateMap<UpdateOptionDto, Option>();
        }
    }
}
