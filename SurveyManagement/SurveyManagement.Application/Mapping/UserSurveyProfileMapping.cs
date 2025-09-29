using AutoMapper;
using SurveyManagement.Application.DTOs.UserSurveyDTOs;
using SurveyManagement.Domain.Entities;

namespace SurveyManagement.Application.Mapping
{
    public class UserSurveyProfileMapping : Profile
    {
        public UserSurveyProfileMapping()
        {
            CreateMap<UserSurvey, UserSurveyDto>().ReverseMap();
            CreateMap<UserSurvey, CreateUserSurveyDto>().ReverseMap();
        }
    }
}
