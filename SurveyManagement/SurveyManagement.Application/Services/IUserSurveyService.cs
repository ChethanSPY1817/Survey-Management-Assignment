using SurveyManagement.Application.DTOs.UserSurveyDTOs;

namespace SurveyManagement.Application.Services
{
    public interface IUserSurveyService
    {
        Task<IEnumerable<UserSurveyDto>> GetAllUserSurveysAsync(); // For admins: only their surveys
        Task<IEnumerable<UserSurveyDto>> GetUserSurveysByCreatorIdAsync(Guid creatorId); // New
        Task<UserSurveyDto?> GetUserSurveyByIdAsync(Guid id);
        Task<UserSurveyDto> CreateUserSurveyAsync(CreateUserSurveyDto dto, Guid createdById); // Updated
        Task UpdateUserSurveyAsync(UserSurveyDto dto);
        Task DeleteUserSurveyAsync(Guid id);
    }
}
