using SurveyManagement.Domain.Entities;

namespace SurveyManagement.Domain.Interfaces
{
    public interface IResponseRepository
    {
        Task<IEnumerable<Response>> GetAllAsync();
        Task<Response?> GetByIdAsync(Guid responseId);
        Task AddAsync(Response response);
        Task UpdateAsync(Response response);
        Task DeleteAsync(Guid responseId);
    }
}
