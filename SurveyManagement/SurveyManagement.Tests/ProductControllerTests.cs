using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveyManagement.Application.Services;
using SurveyManagement.Application.DTOs.ProductDTOs;
using SurveyManagement.Domain.Exceptions;
using System;
using System.Collections.Generic;

namespace SurveyManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _service;

        public ProductController(IProductService service) => _service = service;

        [HttpGet]
        [Authorize(Roles = "Admin,Respondent")]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Respondent")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var product = await _service.GetByIdAsync(id)
                          ?? throw new KeyNotFoundException($"Product with ID {id} not found.");
            return Ok(product);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
        {
            if (dto == null) throw new BadRequestException("Product DTO cannot be null.");

            var createdProduct = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = createdProduct.ProductId }, createdProduct);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ProductDto dto)
        {
            if (id != dto.ProductId) throw new BadRequestException("ID mismatch");

            var existingProduct = await _service.GetByIdAsync(id)
                                  ?? throw new KeyNotFoundException($"Product with ID {id} not found.");

            await _service.UpdateAsync(dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var existingProduct = await _service.GetByIdAsync(id)
                                  ?? throw new KeyNotFoundException($"Product with ID {id} not found.");

            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
