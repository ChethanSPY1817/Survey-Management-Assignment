using AutoMapper;
using SurveyManagement.Domain.Entities;
using SurveyManagement.Application.DTOs.UserProfileDTOs;

namespace SurveyManagement.Application.Mapping
{
    public class UserProfileProfile : Profile
    {
        public UserProfileProfile()
        {
            CreateMap<UserProfile, UserProfileDto>();
            CreateMap<CreateUserProfileDto, UserProfile>()
                .ForMember(dest => dest.UserProfileId, opt => opt.MapFrom(src => Guid.NewGuid()));
        }
    }
}
