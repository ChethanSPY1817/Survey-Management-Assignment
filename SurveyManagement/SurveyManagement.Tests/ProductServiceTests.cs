using AutoMapper;
using Moq;
using NUnit.Framework;
using SurveyManagement.Application.DTOs.ProductDTOs;
using SurveyManagement.Application.Services;
using SurveyManagement.Domain.Entities;
using SurveyManagement.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SurveyManagement.Tests.Services
{
    [TestFixture]
    public class ProductServiceTests
    {
        private Mock<IProductRepository> _repoMock = null!;
        private Mock<IMapper> _mapperMock = null!;
        private ProductService _service = null!;

        [SetUp]
        public void Setup()
        {
            _repoMock = new Mock<IProductRepository>();
            _mapperMock = new Mock<IMapper>();
            _service = new ProductService(_repoMock.Object, _mapperMock.Object);
        }

        [Test]
        public async Task GetAllAsync_ReturnsProducts()
        {
            var products = new List<Product> { new Product { ProductId = Guid.NewGuid(), Name = "Prod1" } };
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(products);
            _mapperMock.Setup(m => m.Map<IEnumerable<ProductDto>>(products)).Returns(new List<ProductDto> { new ProductDto { ProductId = products[0].ProductId } });

            var result = await _service.GetAllAsync();

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Exactly(1).Items);
        }

        [Test]
        public void GetAllAsync_ThrowsKeyNotFoundException_WhenNoProducts()
        {
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Product>());

            Assert.That(async () => await _service.GetAllAsync(),
                        Throws.TypeOf<KeyNotFoundException>());
        }

        [Test]
        public async Task GetByIdAsync_ReturnsProduct_WhenExists()
        {
            var id = Guid.NewGuid();
            var product = new Product { ProductId = id, Name = "Test" };
            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(product);
            _mapperMock.Setup(m => m.Map<ProductDto>(product)).Returns(new ProductDto { ProductId = id });

            var result = await _service.GetByIdAsync(id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.ProductId, Is.EqualTo(id));
        }

        [Test]
        public void GetByIdAsync_ThrowsKeyNotFoundException_WhenNotExists()
        {
            var id = Guid.NewGuid();
            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Product?)null);

            Assert.That(async () => await _service.GetByIdAsync(id),
                        Throws.TypeOf<KeyNotFoundException>());
        }

        [Test]
        public async Task CreateAsync_CreatesProductSuccessfully()
        {
            var dto = new CreateProductDto { Name = "New" };

            // Map DTO to Product with dynamic GUID
            _mapperMock.Setup(m => m.Map<Product>(dto)).Returns((CreateProductDto d) => new Product
            {
                ProductId = Guid.NewGuid(),
                Name = d.Name
            });

            _repoMock.Setup(r => r.AddAsync(It.IsAny<Product>())).Returns(Task.CompletedTask);

            // Map saved Product to ProductDto
            _mapperMock.Setup(m => m.Map<ProductDto>(It.IsAny<Product>())).Returns((Product p) => new ProductDto
            {
                ProductId = p.ProductId,
                Name = p.Name
            });

            var result = await _service.CreateAsync(dto);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ProductId, Is.Not.EqualTo(Guid.Empty));
            Assert.That(result.Name, Is.EqualTo(dto.Name));
        }

        [Test]
        public void CreateAsync_ThrowsInvalidOperationException_WhenDuplicateName()
        {
            var existingProduct = new Product { ProductId = Guid.NewGuid(), Name = "Existing" };
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Product> { existingProduct });
            var dto = new CreateProductDto { Name = "Existing" };

            Assert.That(async () => await _service.CreateAsync(dto),
                        Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public async Task UpdateAsync_UpdatesProductSuccessfully()
        {
            var id = Guid.NewGuid();
            var dto = new ProductDto { ProductId = id, Name = "Updated" };
            var product = new Product { ProductId = id, Name = "Old" };
            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(product);
            _repoMock.Setup(r => r.UpdateAsync(product)).Returns(Task.CompletedTask);

            await _service.UpdateAsync(dto);

            Assert.That(product.Name, Is.EqualTo("Updated"));
        }

        [Test]
        public void UpdateAsync_ThrowsKeyNotFoundException_WhenNotFound()
        {
            var dto = new ProductDto { ProductId = Guid.NewGuid(), Name = "X" };
            _repoMock.Setup(r => r.GetByIdAsync(dto.ProductId)).ReturnsAsync((Product?)null);

            Assert.That(async () => await _service.UpdateAsync(dto),
                        Throws.TypeOf<KeyNotFoundException>());
        }

        [Test]
        public async Task DeleteAsync_DeletesProductSuccessfully()
        {
            var id = Guid.NewGuid();
            var product = new Product { ProductId = id };
            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(product);
            _repoMock.Setup(r => r.DeleteAsync(id)).Returns(Task.CompletedTask);

            Assert.That(async () => await _service.DeleteAsync(id), Throws.Nothing);
        }

        [Test]
        public void DeleteAsync_ThrowsKeyNotFoundException_WhenNotFound()
        {
            var id = Guid.NewGuid();
            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Product?)null);

            Assert.That(async () => await _service.DeleteAsync(id),
                        Throws.TypeOf<KeyNotFoundException>());
        }
    }
}
