using SurveyManagement.Application.DTOs.OptionDTOs;

namespace SurveyManagement.Application.Services
{
    public interface IOptionService
    {
        // Return DTO list for a question
        Task<IEnumerable<OptionDto>> GetAllByQuestionIdAsync(Guid questionId);

        // Return single DTO, throws NotFoundException if missing
        Task<OptionDto> GetByIdAsync(Guid optionId);

        // Return created DTO after creation
        Task<OptionDto> CreateOptionAsync(CreateOptionDto createDto);

        // Return updated DTO (optional) or just ensure update, throws NotFoundException if missing
        Task<OptionDto> UpdateOptionAsync(UpdateOptionDto updateDto);

        // Delete and throw NotFoundException if missing
        Task DeleteOptionAsync(Guid optionId);
    }
}
