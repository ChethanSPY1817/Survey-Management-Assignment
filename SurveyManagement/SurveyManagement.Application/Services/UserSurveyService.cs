using AutoMapper;
using SurveyManagement.Application.DTOs.UserSurveyDTOs;
using SurveyManagement.Domain.Entities;
using SurveyManagement.Domain.Interfaces;

namespace SurveyManagement.Application.Services
{
    public class UserSurveyService : IUserSurveyService
    {
        private readonly IUserSurveyRepository _repository;
        private readonly IMapper _mapper;

        public UserSurveyService(IUserSurveyRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserSurveyDto>> GetAllUserSurveysAsync()
        {
            var surveys = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<UserSurveyDto>>(surveys);
        }

        public async Task<IEnumerable<UserSurveyDto>> GetUserSurveysByCreatorIdAsync(Guid creatorId)
        {
            var surveys = await _repository.GetByCreatorIdAsync(creatorId);
            return _mapper.Map<IEnumerable<UserSurveyDto>>(surveys);
        }

        public async Task<UserSurveyDto?> GetUserSurveyByIdAsync(Guid id)
        {
            var survey = await _repository.GetByIdAsync(id);
            return _mapper.Map<UserSurveyDto?>(survey);
        }

        public async Task<UserSurveyDto> CreateUserSurveyAsync(CreateUserSurveyDto dto, Guid createdById)
        {
            var entity = _mapper.Map<UserSurvey>(dto);
            entity.UserSurveyId = Guid.NewGuid();
            entity.StartedAt = DateTime.UtcNow;
            entity.CreatedById = createdById; // Track the admin who created it

            await _repository.AddAsync(entity);
            return _mapper.Map<UserSurveyDto>(entity);
        }

        public async Task UpdateUserSurveyAsync(UserSurveyDto dto)
        {
            var entity = _mapper.Map<UserSurvey>(dto);
            await _repository.UpdateAsync(entity);
        }

        public async Task DeleteUserSurveyAsync(Guid id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
