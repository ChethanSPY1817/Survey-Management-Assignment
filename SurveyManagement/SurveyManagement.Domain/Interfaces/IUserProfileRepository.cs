using SurveyManagement.Domain.Entities;

namespace SurveyManagement.Domain.Interfaces
{
    public interface IUserProfileRepository
    {
        Task<IEnumerable<UserProfile>> GetAllAsync();
        Task<UserProfile?> GetByIdAsync(Guid userProfileId);
        Task<UserProfile?> GetByUserIdAsync(Guid userId);
        Task AddAsync(UserProfile profile);
        Task UpdateAsync(UserProfile profile);
        Task DeleteAsync(Guid userProfileId);
    }
}
