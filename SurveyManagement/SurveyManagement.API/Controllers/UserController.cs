using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveyManagement.Application.DTOs.UserDTOs;
using SurveyManagement.Application.Services;
using SurveyManagement.Domain.Exceptions;

namespace SurveyManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService) => _userService = userService;

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll() => Ok(await _userService.GetAllUsersAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            var currentUserId = Guid.Parse(User.FindFirst("UserId")?.Value ?? Guid.Empty.ToString());
            if (user.UserId != currentUserId && !User.IsInRole("Admin"))
                throw new UnauthorizedException("You are not allowed to access this user.");

            return Ok(user);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
        {
            var userDto = await _userService.CreateUserAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = userDto.UserId }, userDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UserDto dto)
        {
            if (id != dto.UserId) throw new BadRequestException("User ID mismatch.");
            await _userService.UpdateUserAsync(dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _userService.DeleteUserAsync(id);
            return NoContent();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateUserDto dto)
        {
            dto.Role = "Respondent";
            var user = await _userService.CreateUserAsync(dto);
            return Ok(new { user.UserId, user.Username, user.Email, user.Role });
        }
    }
}
