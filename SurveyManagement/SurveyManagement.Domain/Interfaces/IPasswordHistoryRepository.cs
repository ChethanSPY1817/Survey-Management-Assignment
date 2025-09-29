using SurveyManagement.Domain.Entities;

namespace SurveyManagement.Domain.Interfaces
{
    public interface IPasswordHistoryRepository
    {
        Task AddAsync(PasswordHistory entity);
        Task<IEnumerable<PasswordHistory>> GetByUserIdAsync(Guid userId);
    }
}
