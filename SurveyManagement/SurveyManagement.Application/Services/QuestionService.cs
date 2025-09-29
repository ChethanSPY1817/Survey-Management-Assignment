using AutoMapper;
using SurveyManagement.Application.DTOs.QuestionDTOs;
using SurveyManagement.Domain.Entities;
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
            var questions = await _repo.GetAllBySurveyIdAsync(surveyId);
            return _mapper.Map<IEnumerable<QuestionDto>>(questions);
        }

        public async Task<QuestionDto?> GetByIdAsync(Guid questionId)
        {
            var question = await _repo.GetByIdAsync(questionId);
            if (question == null) return null;
            return _mapper.Map<QuestionDto>(question);
        }

        public async Task<QuestionDto> CreateAsync(CreateQuestionDto createDto)
        {
            var question = _mapper.Map<Question>(createDto);
            question.QuestionId = Guid.NewGuid();
            question.CreatedAt = DateTime.UtcNow; // make sure Question entity has CreatedAt

            await _repo.AddAsync(question);
            return _mapper.Map<QuestionDto>(question);
        }

        public async Task UpdateAsync(UpdateQuestionDto updateDto)
        {
            var question = await _repo.GetByIdAsync(updateDto.QuestionId);
            if (question == null) throw new KeyNotFoundException("Question not found");

            _mapper.Map(updateDto, question);
            await _repo.UpdateAsync(question);
        }

        public async Task DeleteAsync(Guid questionId)
        {
            await _repo.DeleteAsync(questionId);
        }
    }
}
