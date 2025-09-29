using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SurveyManagement.API.Controllers;
using SurveyManagement.Application.Services;
using SurveyManagement.Application.DTOs.ProductDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SurveyManagement.Tests
{
    [TestFixture]
    public class ProductControllerTests
    {
        private Mock<IProductService> _mockService = null!;
        private ProductController _controller = null!;

        [SetUp]
        public void Setup()
        {
            _mockService = new Mock<IProductService>();
            _controller = new ProductController(_mockService.Object);
        }

        [Test]
        public async Task GetAll_ReturnsOkResult_WithProducts()
        {
            _mockService.Setup(s => s.GetAllAsync())
                .ReturnsAsync(new List<ProductDto> { new ProductDto { Name = "Product A" } });

            var result = await _controller.GetAll();
            var okResult = result as OkObjectResult;
            var products = okResult?.Value as IEnumerable<ProductDto>;

            Assert.That(okResult, Is.Not.Null);
            Assert.That(products?.Count() ?? 0, Is.EqualTo(1));
        }

        [Test]
        public async Task GetById_ReturnsOkResult_WhenProductExists()
        {
            var id = Guid.NewGuid();
            _mockService.Setup(s => s.GetByIdAsync(id))
                .ReturnsAsync(new ProductDto { ProductId = id, Name = "Product A" });

            var result = await _controller.GetById(id);
            var okResult = result as OkObjectResult;
            var product = okResult?.Value as ProductDto;

            Assert.That(okResult, Is.Not.Null);
            Assert.That(product?.ProductId, Is.EqualTo(id));
        }

        [Test]
        public async Task GetById_ReturnsNotFound_WhenProductDoesNotExist()
        {
            var id = Guid.NewGuid();
            _mockService.Setup(s => s.GetByIdAsync(id))
                .ReturnsAsync((ProductDto?)null);

            var result = await _controller.GetById(id);
            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public async Task Create_ReturnsCreatedAtAction()
        {
            var createDto = new CreateProductDto { Name = "New Product", Description = "Desc" };

            var result = await _controller.Create(createDto);
            var createdResult = result as CreatedAtActionResult;

            Assert.That(createdResult, Is.Not.Null);
            Assert.That(createdResult?.Value, Is.EqualTo(createDto));
        }

        [Test]
        public async Task Update_ReturnsNoContent_WhenSuccessful()
        {
            var id = Guid.NewGuid();
            var dto = new ProductDto { ProductId = id, Name = "Updated Product" };

            var result = await _controller.Update(id, dto);
            Assert.That(result, Is.TypeOf<NoContentResult>());
        }

        [Test]
        public async Task Update_ReturnsBadRequest_WhenIdMismatch()
        {
            var dto = new ProductDto { ProductId = Guid.NewGuid(), Name = "Mismatch" };
            var result = await _controller.Update(Guid.NewGuid(), dto);
            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task Delete_ReturnsNoContent_WhenSuccessful()
        {
            var id = Guid.NewGuid();
            var result = await _controller.Delete(id);
            Assert.That(result, Is.TypeOf<NoContentResult>());
        }
    }
}
