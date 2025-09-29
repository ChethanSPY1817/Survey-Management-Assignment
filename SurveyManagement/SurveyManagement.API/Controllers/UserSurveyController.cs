using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveyManagement.Application.DTOs.UserSurveyDTOs;
using SurveyManagement.Application.Services;
using System.Security.Claims;

namespace SurveyManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")] // Only admins can access these endpoints
    public class UserSurveyController : ControllerBase
    {
        private readonly IUserSurveyService _service;

        public UserSurveyController(IUserSurveyService service)
        {
            _service = service;
        }

        // Helper method to get current logged-in user's ID from JWT
        private Guid GetCurrentUserId()
        {
            return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var creatorId = GetCurrentUserId();
            var surveys = await _service.GetUserSurveysByCreatorIdAsync(creatorId);
            return Ok(surveys);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var creatorId = GetCurrentUserId();
            var survey = await _service.GetUserSurveyByIdAsync(id);

            if (survey == null || survey.CreatedById != creatorId)
                return NotFound();

            return Ok(survey);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserSurveyDto dto)
        {
            if (dto == null) return BadRequest();

            var createdById = GetCurrentUserId();
            var createdSurvey = await _service.CreateUserSurveyAsync(dto, createdById);

            return CreatedAtAction(nameof(GetById), new { id = createdSurvey.UserSurveyId }, createdSurvey);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UserSurveyDto dto)
        {
            if (id != dto.UserSurveyId) return BadRequest("ID mismatch");

            var creatorId = GetCurrentUserId();
            var existingSurvey = await _service.GetUserSurveyByIdAsync(id);

            if (existingSurvey == null || existingSurvey.CreatedById != creatorId)
                return NotFound();

            await _service.UpdateUserSurveyAsync(dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var creatorId = GetCurrentUserId();
            var existingSurvey = await _service.GetUserSurveyByIdAsync(id);

            if (existingSurvey == null || existingSurvey.CreatedById != creatorId)
                return NotFound();

            await _service.DeleteUserSurveyAsync(id);
            return NoContent();
        }
    }
}
