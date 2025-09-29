using SurveyManagement.Application.DTOs.SurveyDTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SurveyManagement.Application.Services
{
    public interface ISurveyService
    {
        Task<IEnumerable<SurveyDto>> GetAllAsync();
        Task<SurveyDto?> GetByIdAsync(Guid surveyId);

        // Updated: return the created survey
        Task<SurveyDto> CreateAsync(CreateSurveyDto createSurveyDto, Guid currentUserId);

        Task UpdateAsync(SurveyDto surveyDto);
        Task DeleteAsync(Guid surveyId);
    }
}
