using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveyManagement.Application.DTOs.QuestionDTOs;
using SurveyManagement.Application.Services;
using SurveyManagement.Domain.Exceptions;
using System.Security.Claims;

namespace SurveyManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
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
            var questions = await _questionService.GetAllBySurveyIdAsync(surveyId);
            return Ok(questions);
        }

        // GET: api/Question/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var question = await _questionService.GetByIdAsync(id)
                           ?? throw new NotFoundException("Question", id);

            return Ok(question);
        }

        // POST: api/Question
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateQuestionDto dto)
        {
            if (dto == null) throw new BadRequestException("Question data is required.");

            var createdQuestion = await _questionService.CreateAsync(dto);
            return Ok(createdQuestion);
        }

        // PUT: api/Question/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateQuestionDto dto)
        {
            if (id != dto.QuestionId) throw new BadRequestException("Question ID mismatch");

            await _questionService.UpdateAsync(dto);
            return NoContent();
        }

        // DELETE: api/Question/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _questionService.DeleteAsync(id);
            return NoContent();
        }
    }
}
