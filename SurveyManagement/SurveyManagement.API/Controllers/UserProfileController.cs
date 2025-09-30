using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveyManagement.Application.DTOs.UserProfileDTOs;
using SurveyManagement.Application.Services;
using SurveyManagement.Domain.Exceptions;

namespace SurveyManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // All endpoints require authentication
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileService _service;

        public UserProfileController(IUserProfileService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllProfilesAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var profile = await _service.GetProfileByIdAsync(id);

            var currentUserId = Guid.Parse(User.FindFirst("UserId")?.Value ?? Guid.Empty.ToString());
            if (profile.UserId != currentUserId && !User.IsInRole("Admin"))
                throw new UnauthorizedException("You are not allowed to access this profile.");

            return Ok(profile);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(Guid userId)
        {
            var profile = await _service.GetProfileByUserIdAsync(userId);

            var currentUserId = Guid.Parse(User.FindFirst("UserId")?.Value ?? Guid.Empty.ToString());
            if (profile.UserId != currentUserId && !User.IsInRole("Admin"))
                throw new UnauthorizedException("You are not allowed to access this profile.");

            return Ok(profile);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateUserProfileDto createProfileDto)
        {
            var profile = await _service.CreateProfileAsync(createProfileDto);
            return CreatedAtAction(nameof(GetById), new { id = profile.UserProfileId }, profile);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UserProfileDto profileDto)
        {
            if (id != profileDto.UserProfileId)
                throw new BadRequestException("Profile ID mismatch");

            var currentUserId = Guid.Parse(User.FindFirst("UserId")?.Value ?? Guid.Empty.ToString());
            if (profileDto.UserId != currentUserId && !User.IsInRole("Admin"))
                throw new UnauthorizedException("You are not allowed to update this profile.");

            await _service.UpdateProfileAsync(profileDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteProfileAsync(id);
            return NoContent();
        }
    }
}
