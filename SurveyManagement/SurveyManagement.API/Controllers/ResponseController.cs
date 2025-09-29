using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveyManagement.Application.DTOs.ResponseDTOs;
using SurveyManagement.Application.Services;

namespace SurveyManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // All endpoints require authentication
    public class ResponseController : ControllerBase
    {
        private readonly IResponseService _responseService;

        public ResponseController(IResponseService responseService)
        {
            _responseService = responseService;
        }

        // GET: api/Response
        [HttpGet]
        [Authorize(Roles = "Admin")] // Only Admin can view all responses
        public async Task<IActionResult> GetAll()
        {
            var responses = await _responseService.GetAllResponsesAsync();
            return Ok(responses);
        }

        // GET: api/Response/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")] // Only Admin can view a specific response
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await _responseService.GetResponseByIdAsync(id);
            if (response == null) return NotFound();
            return Ok(response);
        }

        // POST: api/Response
        [HttpPost]
        [Authorize(Roles = "Respondent")] // Only Respondent can submit
        public async Task<IActionResult> Create([FromBody] CreateResponseDto createDto)
        {
            if (createDto == null) return BadRequest();

            // Optionally attach UserId from JWT claims
            // createDto.UserId = User.GetUserId();

            await _responseService.CreateResponseAsync(createDto);
            return Ok(createDto);
        }

        // PUT: api/Response/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")] // Only Admin can update if needed
        public async Task<IActionResult> Update(Guid id, [FromBody] ResponseDto responseDto)
        {
            if (id != responseDto.ResponseId) return BadRequest("Response ID mismatch");

            var existingResponse = await _responseService.GetResponseByIdAsync(id);
            if (existingResponse == null) return NotFound();

            await _responseService.UpdateResponseAsync(responseDto);
            return NoContent();
        }

        // DELETE: api/Response/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Only Admin can delete
        public async Task<IActionResult> Delete(Guid id)
        {
            var existingResponse = await _responseService.GetResponseByIdAsync(id);
            if (existingResponse == null) return NotFound();

            await _responseService.DeleteResponseAsync(id);
            return NoContent();
        }
    }
}
