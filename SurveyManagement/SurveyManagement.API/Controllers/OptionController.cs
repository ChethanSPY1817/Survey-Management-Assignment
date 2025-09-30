using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveyManagement.Application.DTOs.OptionDTOs;
using SurveyManagement.Application.Services;
using SurveyManagement.Domain.Exceptions;
using System.Security.Claims;

namespace SurveyManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
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
            var options = await _optionService.GetAllByQuestionIdAsync(questionId);
            return Ok(options);
        }

        // GET: api/Option/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var option = await _optionService.GetByIdAsync(id);
            return Ok(option);
        }

        // POST: api/Option
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateOptionDto createDto)
        {
            if (createDto == null)
                throw new BadRequestException("Option data is required.");

            var createdOption = await _optionService.CreateOptionAsync(createDto);
            return Ok(createdOption);
        }

        // PUT: api/Option/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateOptionDto updateDto)
        {
            if (id != updateDto.OptionId)
                throw new BadRequestException("Option ID mismatch.");

            var updatedOption = await _optionService.UpdateOptionAsync(updateDto);
            return Ok(updatedOption); // Returns updated option in response body
        }

        // DELETE: api/Option/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _optionService.DeleteOptionAsync(id);
            return NoContent();
        }
    }
}
