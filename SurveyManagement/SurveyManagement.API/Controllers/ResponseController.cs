using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveyManagement.Application.DTOs.ResponseDTOs;
using SurveyManagement.Application.Services;
using SurveyManagement.Domain.Exceptions;
using System.Security.Claims;

namespace SurveyManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ResponseController : ControllerBase
    {
        private readonly IResponseService _responseService;

        public ResponseController(IResponseService responseService)
        {
            _responseService = responseService;
        }

        // GET: api/Response
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var responses = await _responseService.GetAllResponsesAsync();
            return Ok(responses);
        }

        // GET: api/Response/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await _responseService.GetResponseByIdAsync(id);
            return Ok(response);
        }

        // POST: api/Response
        [HttpPost]
        [Authorize(Roles = "Respondent")]
        public async Task<IActionResult> Create([FromBody] CreateResponseDto createDto)
        {
            if (createDto == null)
                throw new BadRequestException("Response data is required.");

            // Optionally attach UserId from JWT claims
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? throw new UnauthorizedException("User not authenticated"));

            var createdResponse = await _responseService.CreateResponseAsync(createDto);
            return Ok(createdResponse);
        }

        // PUT: api/Response/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ResponseDto responseDto)
        {
            if (id != responseDto.ResponseId)
                throw new BadRequestException("Response ID mismatch.");

            var updatedResponse = await _responseService.UpdateResponseAsync(responseDto);
            return Ok(updatedResponse);
        }

        // DELETE: api/Response/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _responseService.DeleteResponseAsync(id);
            return NoContent();
        }
    }
}
