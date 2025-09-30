using AutoMapper;
using SurveyManagement.Application.DTOs.SurveyDTOs;
using SurveyManagement.Domain.Entities;
using SurveyManagement.Domain.Exceptions;
using SurveyManagement.Domain.Interfaces;
using SurveyManagement.CrossCutting.Logging;
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
        private readonly IServiceLogger<SurveyService> _logger;

        public SurveyService(
            ISurveyRepository surveyRepository,
            IMapper mapper,
            IServiceLogger<SurveyService> logger)
        {
            _surveyRepository = surveyRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<SurveyDto>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all surveys.");
                var surveys = await _surveyRepository.GetAllAsync();

                if (surveys == null || !surveys.Any())
                {
                    _logger.LogWarning("No surveys found in the database.");
                    return Enumerable.Empty<SurveyDto>();
                }

                _logger.LogInformation($"Fetched {surveys.Count()} surveys successfully.");
                return surveys.Select(s => _mapper.Map<SurveyDto>(s));
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while fetching all surveys.", ex);
                throw;
            }
        }

        public async Task<SurveyDto> GetByIdAsync(Guid surveyId)
        {
            try
            {
                _logger.LogInformation($"Fetching survey with ID: {surveyId}");
                var survey = await _surveyRepository.GetByIdAsync(surveyId);
                if (survey == null)
                {
                    _logger.LogWarning($"Survey with ID {surveyId} not found.");
                    throw new NotFoundException("Survey", surveyId);
                }

                _logger.LogInformation($"Survey with ID {surveyId} fetched successfully.");
                return _mapper.Map<SurveyDto>(survey);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while fetching survey with ID {surveyId}", ex);
                throw;
            }
        }

        public async Task<SurveyDto> CreateAsync(CreateSurveyDto createSurveyDto, Guid currentUserId)
        {
            try
            {
                _logger.LogInformation($"Creating new survey for user ID: {currentUserId}");
                var survey = _mapper.Map<Survey>(createSurveyDto);
                survey.CreatedByUserId = currentUserId;
                survey.CreatedAt = DateTime.UtcNow;

                await _surveyRepository.AddAsync(survey);
                _logger.LogInformation($"Survey created successfully with ID: {survey.SurveyId}");

                return _mapper.Map<SurveyDto>(survey);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while creating survey.", ex);
                throw;
            }
        }

        public async Task UpdateAsync(SurveyDto surveyDto, Guid currentUserId)
        {
            try
            {
                _logger.LogInformation($"Updating survey with ID: {surveyDto.SurveyId}");
                var existingSurvey = await _surveyRepository.GetByIdAsync(surveyDto.SurveyId);

                if (existingSurvey == null)
                {
                    _logger.LogWarning($"Survey with ID {surveyDto.SurveyId} not found.");
                    throw new NotFoundException("Survey", surveyDto.SurveyId);
                }

                if (existingSurvey.CreatedByUserId != currentUserId)
                {
                    _logger.LogWarning($"User {currentUserId} unauthorized to update survey {surveyDto.SurveyId}.");
                    throw new UnauthorizedException("You are not authorized to update this survey.");
                }

                existingSurvey.Title = surveyDto.Title;
                existingSurvey.Description = surveyDto.Description;
                existingSurvey.IsActive = surveyDto.IsActive;
                existingSurvey.ProductId = surveyDto.ProductId;

                await _surveyRepository.UpdateAsync(existingSurvey);
                _logger.LogInformation($"Survey with ID {surveyDto.SurveyId} updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while updating survey with ID {surveyDto.SurveyId}", ex);
                throw;
            }
        }

        public async Task DeleteAsync(Guid surveyId, Guid currentUserId)
        {
            try
            {
                _logger.LogInformation($"Deleting survey with ID: {surveyId}");
                var existingSurvey = await _surveyRepository.GetByIdAsync(surveyId);

                if (existingSurvey == null)
                {
                    _logger.LogWarning($"Survey with ID {surveyId} not found.");
                    throw new NotFoundException("Survey", surveyId);
                }

                if (existingSurvey.CreatedByUserId != currentUserId)
                {
                    _logger.LogWarning($"User {currentUserId} unauthorized to delete survey {surveyId}.");
                    throw new UnauthorizedException("You are not authorized to delete this survey.");
                }

                await _surveyRepository.DeleteAsync(surveyId);
                _logger.LogInformation($"Survey with ID {surveyId} deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while deleting survey with ID {surveyId}", ex);
                throw;
            }
        }
    }
}
