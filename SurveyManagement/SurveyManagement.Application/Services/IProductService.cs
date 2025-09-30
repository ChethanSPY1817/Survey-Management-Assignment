using SurveyManagement.Application.DTOs.ProductDTOs;

namespace SurveyManagement.Application.Services
{
    public interface IProductService
    {
        // Throws KeyNotFoundException if no products found
        Task<IEnumerable<ProductDto>> GetAllAsync();

        // Throws KeyNotFoundException if productId not found
        Task<ProductDto?> GetByIdAsync(Guid productId);

        // Throws ArgumentException if invalid data
        Task<ProductDto> CreateAsync(CreateProductDto dto);

        // Throws KeyNotFoundException if productId not found
        // Throws ArgumentException if invalid data
        Task UpdateAsync(ProductDto dto);

        // Throws KeyNotFoundException if productId not found
        Task DeleteAsync(Guid productId);
    }

}
