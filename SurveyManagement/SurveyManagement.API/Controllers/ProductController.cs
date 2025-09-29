using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveyManagement.Application.Services;
using SurveyManagement.Application.DTOs.ProductDTOs;

namespace SurveyManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _service;

        public ProductController(IProductService service) => _service = service;

        // Both Admins and Respondents can view products
        [HttpGet]
        [Authorize(Roles = "Admin,Respondent")]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Respondent")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var product = await _service.GetByIdAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        // Only Admins can create products
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
        {
            if (dto == null) return BadRequest();

            var createdProduct = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = createdProduct.ProductId }, createdProduct);
        }

        // Only Admins can update products
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ProductDto dto)
        {
            if (id != dto.ProductId) return BadRequest("ID mismatch");

            var existingProduct = await _service.GetByIdAsync(id);
            if (existingProduct == null) return NotFound();

            await _service.UpdateAsync(dto);
            return NoContent();
        }

        // Only Admins can delete products
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var existingProduct = await _service.GetByIdAsync(id);
            if (existingProduct == null) return NotFound();

            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
