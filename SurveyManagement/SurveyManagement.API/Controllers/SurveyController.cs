using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveyManagement.Application.DTOs.SurveyDTOs;
using SurveyManagement.Application.Services;

namespace SurveyManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // All endpoints require authentication
    public class SurveyController : ControllerBase
    {
        private readonly ISurveyService _surveyService;

        public SurveyController(ISurveyService surveyService)
        {
            _surveyService = surveyService;
        }

        // GET: api/Survey
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var surveys = await _surveyService.GetAllAsync();
            return Ok(surveys);
        }

        // GET: api/Survey/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var survey = await _surveyService.GetByIdAsync(id);
            if (survey == null) return NotFound();

            return Ok(survey);
        }

        // POST: api/Survey
        // Controller
        [HttpPost]
        [Authorize(Roles = "Admin")] // Only Admin can create surveys
        public async Task<IActionResult> Create([FromBody] CreateSurveyDto dto)
        {
            if (dto == null) return BadRequest("Survey data is required.");

            // Get current logged-in Admin's ID from JWT
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

            var currentUserId = Guid.Parse(userIdClaim);

            // Pass the creator's ID to the service layer
            var createdSurvey = await _surveyService.CreateAsync(dto, currentUserId);

            // Return only the DTO without exposing CreatedByUserId in Swagger
            var response = new
            {
                createdSurvey.SurveyId,
                createdSurvey.Title,
                createdSurvey.Description,
                createdSurvey.ProductId,
                createdSurvey.IsActive,
                createdSurvey.CreatedAt
            };

            return Ok(response);
        }



        // PUT: api/Survey/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] SurveyDto dto)
        {
            if (id != dto.SurveyId) return BadRequest("ID mismatch");

            var existingSurvey = await _surveyService.GetByIdAsync(id);
            if (existingSurvey == null) return NotFound();

            var currentUserId = Guid.Parse(User.FindFirst("UserId")?.Value ?? Guid.Empty.ToString());
            if (existingSurvey.CreatedByUserId != currentUserId)
                return Forbid();

            await _surveyService.UpdateAsync(dto);
            return NoContent();
        }

        // DELETE: api/Survey/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var existingSurvey = await _surveyService.GetByIdAsync(id);
            if (existingSurvey == null) return NotFound();

            var currentUserId = Guid.Parse(User.FindFirst("UserId")?.Value ?? Guid.Empty.ToString());
            if (existingSurvey.CreatedByUserId != currentUserId)
                return Forbid();

            await _surveyService.DeleteAsync(id);
            return NoContent();
        }
    }
}
