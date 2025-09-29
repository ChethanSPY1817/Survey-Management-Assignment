using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveyManagement.Application.DTOs.UserProfileDTOs;
using SurveyManagement.Application.Services;

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

        // GET: api/UserProfile
        [HttpGet]
        [Authorize(Roles = "Admin")] // Only Admin can view all profiles
        public async Task<IActionResult> GetAll()
        {
            var profiles = await _service.GetAllProfilesAsync();
            return Ok(profiles);
        }

        // GET: api/UserProfile/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var profile = await _service.GetProfileByIdAsync(id);
            if (profile == null) return NotFound();

            // Only Admin or the user themselves can access
            var currentUserId = Guid.Parse(User.FindFirst("UserId")?.Value ?? Guid.Empty.ToString());
            if (profile.UserId != currentUserId && !User.IsInRole("Admin"))
                return Forbid();

            return Ok(profile);
        }

        // GET: api/UserProfile/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(Guid userId)
        {
            var profile = await _service.GetProfileByUserIdAsync(userId);
            if (profile == null) return NotFound();

            var currentUserId = Guid.Parse(User.FindFirst("UserId")?.Value ?? Guid.Empty.ToString());
            if (profile.UserId != currentUserId && !User.IsInRole("Admin"))
                return Forbid();

            return Ok(profile);
        }

        // POST: api/UserProfile
        [HttpPost]
        [Authorize(Roles = "Admin")] // Only Admin can create profiles
        public async Task<IActionResult> Create([FromBody] CreateUserProfileDto createProfileDto)
        {
            if (createProfileDto == null) return BadRequest();

            var profile = await _service.CreateProfileAsync(createProfileDto);
            return CreatedAtAction(nameof(GetById), new { id = profile.UserProfileId }, profile);
        }

        // PUT: api/UserProfile/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UserProfileDto profileDto)
        {
            if (id != profileDto.UserProfileId) return BadRequest("Profile ID mismatch");

            var existing = await _service.GetProfileByIdAsync(id);
            if (existing == null) return NotFound();

            var currentUserId = Guid.Parse(User.FindFirst("UserId")?.Value ?? Guid.Empty.ToString());
            if (existing.UserId != currentUserId && !User.IsInRole("Admin"))
                return Forbid();

            await _service.UpdateProfileAsync(profileDto);
            return NoContent();
        }

        // DELETE: api/UserProfile/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Only Admin can delete profiles
        public async Task<IActionResult> Delete(Guid id)
        {
            var existing = await _service.GetProfileByIdAsync(id);
            if (existing == null) return NotFound();

            await _service.DeleteProfileAsync(id);
            return NoContent();
        }
    }
}
