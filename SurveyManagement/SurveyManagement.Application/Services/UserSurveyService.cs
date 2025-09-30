using AutoMapper;
using SurveyManagement.Application.DTOs.UserSurveyDTOs;
using SurveyManagement.Domain.Entities;
using SurveyManagement.Domain.Exceptions;
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
            if (survey == null)
                return null;

            return _mapper.Map<UserSurveyDto>(survey);
        }

        public async Task<UserSurveyDto> CreateUserSurveyAsync(CreateUserSurveyDto dto, Guid createdById)
        {
            var entity = _mapper.Map<UserSurvey>(dto);
            entity.UserSurveyId = Guid.NewGuid();
            entity.CreatedById = createdById;

            await _repository.AddAsync(entity);
            return _mapper.Map<UserSurveyDto>(entity);
        }

        public async Task UpdateUserSurveyAsync(UserSurveyDto dto)
        {
            var existingEntity = await _repository.GetByIdAsync(dto.UserSurveyId);
            if (existingEntity == null)
                throw new NotFoundException("UserSurvey", dto.UserSurveyId);

            _mapper.Map(dto, existingEntity); // Map only updated fields
            await _repository.UpdateAsync(existingEntity);
        }

        public async Task DeleteUserSurveyAsync(Guid id)
        {
            var existingEntity = await _repository.GetByIdAsync(id);
            if (existingEntity == null)
                throw new NotFoundException("UserSurvey", id);

            await _repository.DeleteAsync(id);
        }
    }
}
