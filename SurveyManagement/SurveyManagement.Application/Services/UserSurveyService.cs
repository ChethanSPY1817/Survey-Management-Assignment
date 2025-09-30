using AutoMapper;
using SurveyManagement.Application.DTOs.UserSurveyDTOs;
using SurveyManagement.Domain.Entities;
using SurveyManagement.Domain.Exceptions;
using SurveyManagement.Domain.Interfaces;
using SurveyManagement.CrossCutting.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SurveyManagement.Application.Services
{
    public class UserSurveyService : IUserSurveyService
    {
        private readonly IUserSurveyRepository _repository;
        private readonly IMapper _mapper;
        private readonly IServiceLogger<UserSurveyService> _logger;

        public UserSurveyService(
            IUserSurveyRepository repository,
            IMapper mapper,
            IServiceLogger<UserSurveyService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<UserSurveyDto>> GetAllUserSurveysAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all user surveys.");
                var surveys = await _repository.GetAllAsync();
                _logger.LogInformation($"Fetched {surveys.Count()} user surveys successfully.");

                return _mapper.Map<IEnumerable<UserSurveyDto>>(surveys);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while fetching all user surveys.", ex);
                throw;
            }
        }

        public async Task<IEnumerable<UserSurveyDto>> GetUserSurveysByCreatorIdAsync(Guid creatorId)
        {
            try
            {
                _logger.LogInformation($"Fetching user surveys created by user ID: {creatorId}");
                var surveys = await _repository.GetByCreatorIdAsync(creatorId);
                _logger.LogInformation($"Fetched {surveys.Count()} surveys created by user ID: {creatorId}");

                return _mapper.Map<IEnumerable<UserSurveyDto>>(surveys);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while fetching surveys for creator ID {creatorId}", ex);
                throw;
            }
        }

        public async Task<UserSurveyDto?> GetUserSurveyByIdAsync(Guid id)
        {
            try
            {
                _logger.LogInformation($"Fetching user survey with ID: {id}");
                var survey = await _repository.GetByIdAsync(id);
                if (survey == null)
                {
                    _logger.LogWarning($"User survey with ID {id} not found.");
                    return null;
                }

                _logger.LogInformation($"User survey with ID {id} fetched successfully.");
                return _mapper.Map<UserSurveyDto>(survey);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while fetching user survey with ID {id}", ex);
                throw;
            }
        }

        public async Task<UserSurveyDto> CreateUserSurveyAsync(CreateUserSurveyDto dto, Guid createdById)
        {
            try
            {
                _logger.LogInformation($"Creating new user survey by user ID: {createdById}");
                var entity = _mapper.Map<UserSurvey>(dto);
                entity.UserSurveyId = Guid.NewGuid();
                entity.CreatedById = createdById;

                await _repository.AddAsync(entity);
                _logger.LogInformation($"User survey created successfully with ID: {entity.UserSurveyId}");

                return _mapper.Map<UserSurveyDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while creating new user survey.", ex);
                throw;
            }
        }

        public async Task UpdateUserSurveyAsync(UserSurveyDto dto)
        {
            try
            {
                _logger.LogInformation($"Updating user survey with ID: {dto.UserSurveyId}");
                var existingEntity = await _repository.GetByIdAsync(dto.UserSurveyId);
                if (existingEntity == null)
                {
                    _logger.LogWarning($"User survey with ID {dto.UserSurveyId} not found.");
                    throw new NotFoundException("UserSurvey", dto.UserSurveyId);
                }

                _mapper.Map(dto, existingEntity);
                await _repository.UpdateAsync(existingEntity);
                _logger.LogInformation($"User survey with ID {dto.UserSurveyId} updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while updating user survey with ID {dto.UserSurveyId}", ex);
                throw;
            }
        }

        public async Task DeleteUserSurveyAsync(Guid id)
        {
            try
            {
                _logger.LogInformation($"Deleting user survey with ID: {id}");
                var existingEntity = await _repository.GetByIdAsync(id);
                if (existingEntity == null)
                {
                    _logger.LogWarning($"User survey with ID {id} not found.");
                    throw new NotFoundException("UserSurvey", id);
                }

                await _repository.DeleteAsync(id);
                _logger.LogInformation($"User survey with ID {id} deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while deleting user survey with ID {id}", ex);
                throw;
            }
        }
    }
}
