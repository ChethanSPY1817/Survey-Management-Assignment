using SurveyManagement.Domain.Entities;

namespace SurveyManagement.Domain.Interfaces
{
    public interface IUserSurveyRepository
    {
        Task<IEnumerable<UserSurvey>> GetAllAsync();
        Task<IEnumerable<UserSurvey>> GetByCreatorIdAsync(Guid creatorId); // New
        Task<UserSurvey?> GetByIdAsync(Guid id);
        Task AddAsync(UserSurvey entity);
        Task UpdateAsync(UserSurvey entity);
        Task DeleteAsync(Guid id);
    }
}
