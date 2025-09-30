using Microsoft.AspNetCore.Mvc;
using SurveyManagement.Application.DTOs.AuthDTOs;
using SurveyManagement.Application.Services;
using SurveyManagement.Domain.Exceptions;

namespace SurveyManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // LOGIN
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (loginDto == null)
                throw new BadRequestException("Login data is required.");

            var result = await _authService.LoginAsync(loginDto);
            return Ok(result);
        }

        // REGISTER
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (registerDto == null)
                throw new BadRequestException("Registration data is required.");

            var result = await _authService.RegisterAsync(registerDto);
            return Ok(result);
        }
    }
}
