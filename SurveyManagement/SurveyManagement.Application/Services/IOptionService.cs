using SurveyManagement.Application.DTOs.OptionDTOs;

namespace SurveyManagement.Application.Services
{
    public interface IOptionService
    {
        Task<IEnumerable<OptionDto>> GetAllByQuestionIdAsync(Guid questionId);
        Task<OptionDto?> GetByIdAsync(Guid optionId);
        Task CreateOptionAsync(CreateOptionDto createDto);
        Task UpdateOptionAsync(UpdateOptionDto updateDto);
        Task DeleteOptionAsync(Guid optionId);
    }
}
