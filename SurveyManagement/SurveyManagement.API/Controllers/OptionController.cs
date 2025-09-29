using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveyManagement.Application.DTOs.OptionDTOs;
using SurveyManagement.Application.Services;

namespace SurveyManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // All endpoints require authentication
    public class OptionController : ControllerBase
    {
        private readonly IOptionService _optionService;

        public OptionController(IOptionService optionService)
        {
            _optionService = optionService;
        }

        // GET: api/Option/question/{questionId}
        [HttpGet("question/{questionId}")]
        public async Task<IActionResult> GetAllByQuestionId(Guid questionId)
        {
            // Both Admins and Respondents can view options
            var options = await _optionService.GetAllByQuestionIdAsync(questionId);
            return Ok(options);
        }

        // GET: api/Option/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var option = await _optionService.GetByIdAsync(id);
            if (option == null) return NotFound();
            return Ok(option);
        }

        // POST: api/Option
        [HttpPost]
        [Authorize(Roles = "Admin")] // Only Admin can create
        public async Task<IActionResult> Create([FromBody] CreateOptionDto createDto)
        {
            if (createDto == null) return BadRequest();

            // Optional: check if admin owns the question's survey
            // var userId = User.GetUserId();
            // if (!await _optionService.IsQuestionOwner(createDto.QuestionId, userId)) return Forbid();

            await _optionService.CreateOptionAsync(createDto);
            return Ok(createDto);
        }

        // PUT: api/Option/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")] // Only Admin can update
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateOptionDto updateDto)
        {
            if (id != updateDto.OptionId) return BadRequest("Option ID mismatch");

            // Optional ownership check
            // var userId = User.GetUserId();
            // if (!await _optionService.IsQuestionOwner(updateDto.QuestionId, userId)) return Forbid();

            await _optionService.UpdateOptionAsync(updateDto);
            return NoContent();
        }

        // DELETE: api/Option/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Only Admin can delete
        public async Task<IActionResult> Delete(Guid id)
        {
            // Optional ownership check
            // var userId = User.GetUserId();
            // if (!await _optionService.IsOptionOwner(id, userId)) return Forbid();

            await _optionService.DeleteOptionAsync(id);
            return NoContent();
        }
    }
}
