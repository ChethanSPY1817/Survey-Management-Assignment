using AutoMapper;
using SurveyManagement.Domain.Entities;
using SurveyManagement.Application.DTOs.ProductDTOs;

namespace SurveyManagement.Application.Mapping
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<CreateProductDto, Product>();
        }
    }
}
