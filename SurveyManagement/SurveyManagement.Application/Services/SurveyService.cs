using AutoMapper;
using SurveyManagement.Application.DTOs.SurveyDTOs;
using SurveyManagement.Domain.Entities;
using SurveyManagement.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SurveyManagement.Application.Services
{
    public class SurveyService : ISurveyService
    {
        private readonly ISurveyRepository _surveyRepository;
        private readonly IMapper _mapper;

        public SurveyService(ISurveyRepository surveyRepository, IMapper mapper)
        {
            _surveyRepository = surveyRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SurveyDto>> GetAllAsync()
        {
            var surveys = await _surveyRepository.GetAllAsync();
            return surveys.Select(s => _mapper.Map<SurveyDto>(s));
        }

        public async Task<SurveyDto?> GetByIdAsync(Guid surveyId)
        {
            var survey = await _surveyRepository.GetByIdAsync(surveyId);
            if (survey == null) return null;
            return _mapper.Map<SurveyDto>(survey);
        }

        // Updated: set CreatedByUserId externally from controller
        // Service
        public async Task<SurveyDto> CreateAsync(CreateSurveyDto createSurveyDto, Guid currentUserId)
        {
            var survey = _mapper.Map<Survey>(createSurveyDto);

            // Set the creator here
            survey.CreatedByUserId = currentUserId;
            survey.CreatedAt = DateTime.UtcNow;

            await _surveyRepository.AddAsync(survey);

            return _mapper.Map<SurveyDto>(survey);
        }


        public async Task UpdateAsync(SurveyDto surveyDto)
        {
            var existingSurvey = await _surveyRepository.GetByIdAsync(surveyDto.SurveyId);
            if (existingSurvey == null) throw new Exception("Survey not found");

            // Map changes from DTO to entity, preserve FK relationships
            existingSurvey.Title = surveyDto.Title;
            existingSurvey.Description = surveyDto.Description;
            existingSurvey.IsActive = surveyDto.IsActive;
            existingSurvey.ProductId = surveyDto.ProductId;

            await _surveyRepository.UpdateAsync(existingSurvey);
        }

        public async Task DeleteAsync(Guid surveyId)
        {
            await _surveyRepository.DeleteAsync(surveyId);
        }
    }
}
