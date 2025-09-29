using SurveyManagement.Application.DTOs.UserDTOs;

namespace SurveyManagement.Application.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto?> GetUserByIdAsync(Guid userId);
        Task<UserDto> CreateUserAsync(CreateUserDto createUserDto);
        Task UpdateUserAsync(UserDto userDto);
        Task DeleteUserAsync(Guid userId);
    }
}
