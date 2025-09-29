using SurveyManagement.Domain.Entities;

namespace SurveyManagement.Domain.Interfaces
{
    public interface IQuestionRepository
    {
        Task<IEnumerable<Question>> GetAllBySurveyIdAsync(Guid surveyId);
        Task<Question?> GetByIdAsync(Guid questionId);
        Task AddAsync(Question question);
        Task UpdateAsync(Question question);
        Task DeleteAsync(Guid questionId);
    }
}
