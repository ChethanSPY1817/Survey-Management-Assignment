using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveyManagement.Application.DTOs.UserSurveyDTOs;
using SurveyManagement.Application.Services;
using SurveyManagement.Domain.Exceptions;
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
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                throw new UnauthorizedException("User not authenticated");

            return Guid.Parse(userIdClaim);
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
                throw new NotFoundException("UserSurvey", id);

            return Ok(survey);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserSurveyDto dto)
        {
            if (dto == null)
                throw new BadRequestException("Invalid survey data");

            var createdById = GetCurrentUserId();
            var createdSurvey = await _service.CreateUserSurveyAsync(dto, createdById);

            return CreatedAtAction(nameof(GetById), new { id = createdSurvey.UserSurveyId }, createdSurvey);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UserSurveyDto dto)
        {
            if (id != dto.UserSurveyId)
                throw new BadRequestException("ID mismatch");

            var creatorId = GetCurrentUserId();
            var existingSurvey = await _service.GetUserSurveyByIdAsync(id);

            if (existingSurvey == null || existingSurvey.CreatedById != creatorId)
                throw new NotFoundException("UserSurvey", id);

            await _service.UpdateUserSurveyAsync(dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var creatorId = GetCurrentUserId();
            var existingSurvey = await _service.GetUserSurveyByIdAsync(id);

            if (existingSurvey == null || existingSurvey.CreatedById != creatorId)
                throw new NotFoundException("UserSurvey", id);

            await _service.DeleteUserSurveyAsync(id);
            return NoContent();
        }
    }
}
