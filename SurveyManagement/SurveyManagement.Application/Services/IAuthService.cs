using SurveyManagement.Application.DTOs.AuthDTOs;

namespace SurveyManagement.Application.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto); // new
    }
}
