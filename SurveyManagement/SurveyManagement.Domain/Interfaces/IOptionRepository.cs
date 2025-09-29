using SurveyManagement.Domain.Entities;

namespace SurveyManagement.Domain.Interfaces
{
    public interface IOptionRepository
    {
        Task<IEnumerable<Option>> GetAllByQuestionIdAsync(Guid questionId);
        Task<Option?> GetByIdAsync(Guid optionId);
        Task AddAsync(Option option);
        Task UpdateAsync(Option option);
        Task DeleteAsync(Guid optionId);
    }
}
