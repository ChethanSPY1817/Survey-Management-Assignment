using AutoMapper;
using SurveyManagement.Application.DTOs.QuestionDTOs;
using SurveyManagement.Domain.Entities;
using SurveyManagement.Domain.Exceptions;
using SurveyManagement.Domain.Interfaces;
using SurveyManagement.CrossCutting.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SurveyManagement.Application.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly IQuestionRepository _repo;
        private readonly IMapper _mapper;
        private readonly IServiceLogger<QuestionService> _logger;

        public QuestionService(
            IQuestionRepository repo,
            IMapper mapper,
            IServiceLogger<QuestionService> logger)
        {
            _repo = repo;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<QuestionDto>> GetAllBySurveyIdAsync(Guid surveyId)
        {
            try
            {
                _logger.LogInformation($"Fetching all questions for survey ID: {surveyId}");

                var surveyExists = await _repo.SurveyExistsAsync(surveyId);
                if (!surveyExists)
                {
                    _logger.LogWarning($"Survey ID {surveyId} does not exist.");
                    throw new BadRequestException("Invalid Survey Id");
                }

                var questions = await _repo.GetAllBySurveyIdAsync(surveyId);
                _logger.LogInformation($"Fetched {questions.Count()} questions for survey ID: {surveyId}");

                return _mapper.Map<IEnumerable<QuestionDto>>(questions);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while fetching questions for survey ID {surveyId}", ex);
                throw;
            }
        }

        public async Task<QuestionDto> GetByIdAsync(Guid questionId)
        {
            try
            {
                _logger.LogInformation($"Fetching question with ID: {questionId}");

                var question = await _repo.GetByIdAsync(questionId)
                               ?? throw new NotFoundException("Question", questionId);

                _logger.LogInformation($"Question with ID {questionId} fetched successfully.");
                return _mapper.Map<QuestionDto>(question);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while fetching question with ID {questionId}", ex);
                throw;
            }
        }

        public async Task<QuestionDto> CreateAsync(CreateQuestionDto createDto)
        {
            try
            {
                _logger.LogInformation($"Creating new question for survey ID: {createDto.SurveyId}");

                var surveyExists = await _repo.SurveyExistsAsync(createDto.SurveyId);
                if (!surveyExists)
                {
                    _logger.LogWarning($"Survey ID {createDto.SurveyId} does not exist.");
                    throw new BadRequestException("Invalid Survey Id");
                }

                var question = _mapper.Map<Question>(createDto);
                question.QuestionId = Guid.NewGuid();
                question.CreatedAt = DateTime.UtcNow;

                await _repo.AddAsync(question);
                _logger.LogInformation($"Question created successfully with ID: {question.QuestionId}");

                return _mapper.Map<QuestionDto>(question);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while creating a new question.", ex);
                throw;
            }
        }

        public async Task UpdateAsync(UpdateQuestionDto updateDto)
        {
            try
            {
                _logger.LogInformation($"Updating question with ID: {updateDto.QuestionId}");

                var question = await _repo.GetByIdAsync(updateDto.QuestionId)
                               ?? throw new NotFoundException("Question", updateDto.QuestionId);

                _mapper.Map(updateDto, question);
                await _repo.UpdateAsync(question);

                _logger.LogInformation($"Question with ID {updateDto.QuestionId} updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while updating question with ID {updateDto.QuestionId}", ex);
                throw;
            }
        }

        public async Task DeleteAsync(Guid questionId)
        {
            try
            {
                _logger.LogInformation($"Deleting question with ID: {questionId}");

                var question = await _repo.GetByIdAsync(questionId)
                               ?? throw new NotFoundException("Question", questionId);

                await _repo.DeleteAsync(questionId);
                _logger.LogInformation($"Question with ID {questionId} deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while deleting question with ID {questionId}", ex);
                throw;
            }
        }
    }
}
