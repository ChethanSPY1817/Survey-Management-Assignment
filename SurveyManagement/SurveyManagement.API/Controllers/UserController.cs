using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveyManagement.Application.Services;
using SurveyManagement.Application.DTOs.UserDTOs;

namespace SurveyManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // All endpoints require authentication
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/User
        [HttpGet]
        [Authorize(Roles = "Admin")] // Only Admin can view all users
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        // GET: api/User/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound();

            // Optional: Allow users to only access their own info
            var currentUserId = Guid.Parse(User.FindFirst("UserId")?.Value ?? Guid.Empty.ToString());
            if (user.UserId != currentUserId && !User.IsInRole("Admin"))
                return Forbid();

            return Ok(user);
        }

        // POST: api/User
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateUserDto createUserDto)
        {
            var userDto = await _userService.CreateUserAsync(createUserDto);
            return CreatedAtAction(nameof(GetById), new { id = userDto.UserId }, userDto);
        }


        // PUT: api/User/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")] // Only Admin can update users
        public async Task<IActionResult> Update(Guid id, [FromBody] UserDto userDto)
        {
            if (id != userDto.UserId) return BadRequest("User ID mismatch");

            var existingUser = await _userService.GetUserByIdAsync(id);
            if (existingUser == null) return NotFound();

            await _userService.UpdateUserAsync(userDto);
            return NoContent();
        }

        // DELETE: api/User/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Only Admin can delete users
        public async Task<IActionResult> Delete(Guid id)
        {
            var existingUser = await _userService.GetUserByIdAsync(id);
            if (existingUser == null) return NotFound();

            await _userService.DeleteUserAsync(id);
            return NoContent();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateUserDto dto)
        {
            // Ensure only Respondent role is allowed via public register
            dto.Role = "Respondent";

            var user = await _userService.CreateUserAsync(dto);
            return Ok(new { user.UserId, user.Username, user.Email, user.Role });
        }


    }
}
