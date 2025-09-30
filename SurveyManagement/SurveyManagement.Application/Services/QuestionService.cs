using AutoMapper;
using SurveyManagement.Application.DTOs.QuestionDTOs;
using SurveyManagement.Domain.Entities;
using SurveyManagement.Domain.Exceptions;
using SurveyManagement.Domain.Interfaces;

namespace SurveyManagement.Application.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly IQuestionRepository _repo;
        private readonly IMapper _mapper;

        public QuestionService(IQuestionRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<QuestionDto>> GetAllBySurveyIdAsync(Guid surveyId)
        {
            // Check if the survey exists first
            var surveyExists = await _repo.SurveyExistsAsync(surveyId); // You need to implement this method in repository
            if (!surveyExists)
                throw new BadRequestException("Invalid Survey Id"); // Custom exception handled by middleware

            var questions = await _repo.GetAllBySurveyIdAsync(surveyId);
            return _mapper.Map<IEnumerable<QuestionDto>>(questions);
        }


        public async Task<QuestionDto> GetByIdAsync(Guid questionId)
        {
            var question = await _repo.GetByIdAsync(questionId)
                           ?? throw new NotFoundException("Question", questionId);

            return _mapper.Map<QuestionDto>(question);
        }

        public async Task<QuestionDto> CreateAsync(CreateQuestionDto createDto)
        {
            // Check if the survey exists
            var surveyExists = await _repo.SurveyExistsAsync(createDto.SurveyId);
            if (!surveyExists)
                throw new BadRequestException("Invalid Survey Id"); // Will be returned in response body

            var question = _mapper.Map<Question>(createDto);
            question.QuestionId = Guid.NewGuid();
            question.CreatedAt = DateTime.UtcNow;

            await _repo.AddAsync(question);
            return _mapper.Map<QuestionDto>(question);
        }


        public async Task UpdateAsync(UpdateQuestionDto updateDto)
        {
            var question = await _repo.GetByIdAsync(updateDto.QuestionId)
                           ?? throw new NotFoundException("Question", updateDto.QuestionId);

            _mapper.Map(updateDto, question);
            await _repo.UpdateAsync(question);
        }

        public async Task DeleteAsync(Guid questionId)
        {
            var question = await _repo.GetByIdAsync(questionId)
                           ?? throw new NotFoundException("Question", questionId);

            await _repo.DeleteAsync(questionId);
        }
    }
}
