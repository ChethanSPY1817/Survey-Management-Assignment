using AutoMapper;
using SurveyManagement.Domain.Interfaces;
using SurveyManagement.Domain.Entities;
using SurveyManagement.Application.DTOs.ProductDTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SurveyManagement.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repo;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            var products = await _repo.GetAllAsync();

            if (products == null || !products.Any())
                throw new KeyNotFoundException("No products found in the database.");

            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<ProductDto?> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid product ID provided.");

            var product = await _repo.GetByIdAsync(id);

            if (product == null)
                throw new KeyNotFoundException($"Product with ID '{id}' not found.");

            return _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "Product data must be provided.");

            // Optional: check for duplicate product by name
            var existingProducts = await _repo.GetAllAsync();
            if (existingProducts.Any(p => p.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException($"A product with name '{dto.Name}' already exists.");

            var product = _mapper.Map<Product>(dto);
            product.ProductId = Guid.NewGuid();

            await _repo.AddAsync(product);

            return _mapper.Map<ProductDto>(product);
        }

        public async Task UpdateAsync(ProductDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "Product data must be provided.");

            if (dto.ProductId == Guid.Empty)
                throw new ArgumentException("Invalid product ID provided.");

            var existingProduct = await _repo.GetByIdAsync(dto.ProductId);
            if (existingProduct == null)
                throw new KeyNotFoundException($"Product with ID '{dto.ProductId}' not found.");

            // Map updated fields
            existingProduct.Name = dto.Name;
            existingProduct.Description = dto.Description;

            await _repo.UpdateAsync(existingProduct);
        }

        public async Task DeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid product ID provided.");

            var existingProduct = await _repo.GetByIdAsync(id);
            if (existingProduct == null)
                throw new KeyNotFoundException($"Product with ID '{id}' not found.");

            await _repo.DeleteAsync(id);
        }
    }
}
