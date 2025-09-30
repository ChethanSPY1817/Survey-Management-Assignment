using SurveyManagement.Application.DTOs.SurveyDTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SurveyManagement.Application.Services
{
    public interface ISurveyService
    {
        Task<IEnumerable<SurveyDto>> GetAllAsync();

        Task<SurveyDto> GetByIdAsync(Guid surveyId);
        // ✅ No longer nullable, throws NotFoundException if not found

        Task<SurveyDto> CreateAsync(CreateSurveyDto createSurveyDto, Guid currentUserId);

        Task UpdateAsync(SurveyDto surveyDto, Guid currentUserId);
        // ✅ Throws NotFoundException or UnauthorizedException if not allowed

        Task DeleteAsync(Guid surveyId, Guid currentUserId);
        // ✅ Throws NotFoundException or UnauthorizedException if not allowed
    }
}
