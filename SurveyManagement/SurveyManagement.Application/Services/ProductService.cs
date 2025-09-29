using AutoMapper;
using SurveyManagement.Domain.Interfaces;
using SurveyManagement.Domain.Entities;
using SurveyManagement.Application.DTOs.ProductDTOs;

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
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<ProductDto?> GetByIdAsync(Guid id)
        {
            var product = await _repo.GetByIdAsync(id);
            return _mapper.Map<ProductDto>(product);
        }

        // ✅ Updated to return ProductDto
        public async Task<ProductDto> CreateAsync(CreateProductDto dto)
        {
            var product = _mapper.Map<Product>(dto);
            product.ProductId = Guid.NewGuid();

            await _repo.AddAsync(product);

            return _mapper.Map<ProductDto>(product);
        }

        public async Task UpdateAsync(ProductDto dto)
        {
            var product = _mapper.Map<Product>(dto);
            await _repo.UpdateAsync(product);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _repo.DeleteAsync(id);
        }
    }
}
