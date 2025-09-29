using Microsoft.AspNetCore.Mvc;
using SurveyManagement.Application.DTOs.UserProfileDTOs;
using SurveyManagement.Application.Services;

namespace SurveyManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileService _service;

        public UserProfileController(IUserProfileService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var profiles = await _service.GetAllProfilesAsync();
            return Ok(profiles);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var profile = await _service.GetProfileByIdAsync(id);
            if (profile == null) return NotFound();
            return Ok(profile);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(Guid userId)
        {
            var profile = await _service.GetProfileByUserIdAsync(userId);
            if (profile == null) return NotFound();
            return Ok(profile);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserProfileDto createProfileDto)
        {
            if (createProfileDto == null) return BadRequest();

            var profile = await _service.CreateProfileAsync(createProfileDto);
            return CreatedAtAction(nameof(GetById), new { id = profile.UserProfileId }, profile);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UserProfileDto profileDto)
        {
            if (id != profileDto.UserProfileId) return BadRequest("Profile ID mismatch");

            var existing = await _service.GetProfileByIdAsync(id);
            if (existing == null) return NotFound();

            await _service.UpdateProfileAsync(profileDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var existing = await _service.GetProfileByIdAsync(id);
            if (existing == null) return NotFound();

            await _service.DeleteProfileAsync(id);
            return NoContent();
        }
    }
}
