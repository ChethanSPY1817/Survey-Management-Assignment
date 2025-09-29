using SurveyManagement.Application.DTOs.ProductDTOs;

namespace SurveyManagement.Application.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllAsync();
        Task<ProductDto?> GetByIdAsync(Guid productId);
        Task<ProductDto> CreateAsync(CreateProductDto dto);
        Task UpdateAsync(ProductDto dto);
        Task DeleteAsync(Guid productId);
    }
}
