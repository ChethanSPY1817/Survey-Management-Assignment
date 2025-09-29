using SurveyManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SurveyManagement.Domain.Interfaces
{
    public interface ISurveyRepository
    {
        Task<IEnumerable<Survey>> GetAllAsync();
        Task<Survey?> GetByIdAsync(Guid surveyId);
        Task AddAsync(Survey survey);
        Task UpdateAsync(Survey survey);
        Task DeleteAsync(Guid surveyId);
    }
}
