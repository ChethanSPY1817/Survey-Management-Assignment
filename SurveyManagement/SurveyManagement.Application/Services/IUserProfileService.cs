using SurveyManagement.Application.DTOs.UserProfileDTOs;

namespace SurveyManagement.Application.Services
{
    public interface IUserProfileService
    {
        Task<IEnumerable<UserProfileDto>> GetAllProfilesAsync();
        Task<UserProfileDto?> GetProfileByIdAsync(Guid profileId);
        Task<UserProfileDto?> GetProfileByUserIdAsync(Guid userId);
        Task<UserProfileDto> CreateProfileAsync(CreateUserProfileDto createProfileDto);
        Task UpdateProfileAsync(UserProfileDto profileDto);
        Task DeleteProfileAsync(Guid profileId);
    }
}
