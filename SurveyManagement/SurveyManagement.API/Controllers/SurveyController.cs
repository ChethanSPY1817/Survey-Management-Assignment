using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveyManagement.Application.DTOs.SurveyDTOs;
using SurveyManagement.Application.Services;
using SurveyManagement.Domain.Exceptions;
using System.Security.Claims;

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

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _surveyService.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var survey = await _surveyService.GetByIdAsync(id);
            return Ok(survey);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateSurveyDto dto)
        {
            var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
                                  ?? throw new UnauthorizedException("User not authenticated"));

            var createdSurvey = await _surveyService.CreateAsync(dto, currentUserId);

            return Ok(new
            {
                createdSurvey.SurveyId,
                createdSurvey.Title,
                createdSurvey.Description,
                createdSurvey.ProductId,
                createdSurvey.IsActive,
                createdSurvey.CreatedAt
            });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] SurveyDto dto)
        {
            if (id != dto.SurveyId)
                throw new BadRequestException("ID mismatch");

            var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
                                  ?? throw new UnauthorizedException("User not authenticated"));

            await _surveyService.UpdateAsync(dto, currentUserId);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
                                  ?? throw new UnauthorizedException("User not authenticated"));

            await _surveyService.DeleteAsync(id, currentUserId);
            return NoContent();
        }
    }
}
