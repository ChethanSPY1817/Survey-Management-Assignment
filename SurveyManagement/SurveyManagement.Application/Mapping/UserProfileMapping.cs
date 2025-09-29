using AutoMapper;
using SurveyManagement.Application.DTOs.UserDTOs;
using SurveyManagement.Domain.Entities;

namespace SurveyManagement.Application.Mapping
{
    public class UserProfileMapping : Profile
    {
        public UserProfileMapping()
        {
            // Read/Update mapping
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Profile != null ? src.Profile.FirstName : ""))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Profile != null ? src.Profile.LastName : ""))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Profile != null ? src.Profile.Phone : null))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Profile != null ? src.Profile.Address : null));

            CreateMap<UserDto, User>()
                .ForMember(dest => dest.Profile, opt => opt.MapFrom(src => new UserProfile
                {
                    FirstName = src.FirstName,
                    LastName = src.LastName,
                    Phone = src.Phone,
                    Address = src.Address
                }));

            // Create mapping
            CreateMap<CreateUserDto, User>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(_ => Guid.NewGuid()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(_ => UserRole.Respondent))
                .ForMember(dest => dest.Profile, opt => opt.MapFrom(src => new UserProfile
                {
                    FirstName = src.FirstName,
                    LastName = src.LastName,
                    Phone = src.Phone,
                    Address = src.Address
                }));
        }
    }
}
