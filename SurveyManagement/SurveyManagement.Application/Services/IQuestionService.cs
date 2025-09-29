using SurveyManagement.Application.DTOs.QuestionDTOs;

namespace SurveyManagement.Application.Services
{
    public interface IQuestionService
    {
        Task<IEnumerable<QuestionDto>> GetAllBySurveyIdAsync(Guid surveyId);
        Task<QuestionDto?> GetByIdAsync(Guid questionId);
        Task<QuestionDto> CreateAsync(CreateQuestionDto createDto);
        Task UpdateAsync(UpdateQuestionDto updateDto);
        Task DeleteAsync(Guid questionId);
    }
}
