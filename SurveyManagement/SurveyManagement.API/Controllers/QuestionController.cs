using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveyManagement.Application.DTOs.QuestionDTOs;
using SurveyManagement.Application.Services;

namespace SurveyManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // All endpoints require authentication
    public class QuestionController : ControllerBase
    {
        private readonly IQuestionService _questionService;

        public QuestionController(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        // GET: api/Question/survey/{surveyId}
        [HttpGet("survey/{surveyId}")]
        public async Task<IActionResult> GetAllBySurveyId(Guid surveyId)
        {
            // Both Admins and Respondents can view questions
            var questions = await _questionService.GetAllBySurveyIdAsync(surveyId);
            return Ok(questions);
        }

        // GET: api/Question/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var question = await _questionService.GetByIdAsync(id);
            if (question == null) return NotFound();
            return Ok(question);
        }

        // POST: api/Question
        [HttpPost]
        [Authorize(Roles = "Admin")] // Only Admin can create
        public async Task<IActionResult> Create([FromBody] CreateQuestionDto createDto)
        {
            if (createDto == null) return BadRequest();

            // Optional: check if the authenticated admin owns the survey
            // var userId = User.GetUserId(); // Implement an extension to get JWT userId
            // if (!await _questionService.IsSurveyOwner(createDto.SurveyId, userId)) return Forbid();

            var createdQuestion = await _questionService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = createdQuestion.QuestionId }, createdQuestion);
        }

        // PUT: api/Question/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")] // Only Admin can update
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateQuestionDto updateDto)
        {
            if (id != updateDto.QuestionId) return BadRequest("Question ID mismatch");

            var existingQuestion = await _questionService.GetByIdAsync(id);
            if (existingQuestion == null) return NotFound();

            // Optional: check if the authenticated admin owns the survey
            // var userId = User.GetUserId();
            // if (!await _questionService.IsSurveyOwner(existingQuestion.SurveyId, userId)) return Forbid();

            await _questionService.UpdateAsync(updateDto);
            return NoContent();
        }

        // DELETE: api/Question/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Only Admin can delete
        public async Task<IActionResult> Delete(Guid id)
        {
            var existingQuestion = await _questionService.GetByIdAsync(id);
            if (existingQuestion == null) return NotFound();

            // Optional: check if the authenticated admin owns the survey
            // var userId = User.GetUserId();
            // if (!await _questionService.IsSurveyOwner(existingQuestion.SurveyId, userId)) return Forbid();

            await _questionService.DeleteAsync(id);
            return NoContent();
        }
    }
}
