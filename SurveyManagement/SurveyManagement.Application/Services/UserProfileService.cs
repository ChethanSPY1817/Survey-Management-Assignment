using AutoMapper;
using SurveyManagement.Application.DTOs.UserProfileDTOs;
using SurveyManagement.Domain.Entities;
using SurveyManagement.Domain.Interfaces;

namespace SurveyManagement.Application.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IUserProfileRepository _repository;
        private readonly IMapper _mapper;

        public UserProfileService(IUserProfileRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserProfileDto>> GetAllProfilesAsync()
        {
            var profiles = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<UserProfileDto>>(profiles);
        }

        public async Task<UserProfileDto?> GetProfileByIdAsync(Guid profileId)
        {
            var profile = await _repository.GetByIdAsync(profileId);
            return _mapper.Map<UserProfileDto?>(profile);
        }

        public async Task<UserProfileDto?> GetProfileByUserIdAsync(Guid userId)
        {
            var profile = await _repository.GetByUserIdAsync(userId);
            return _mapper.Map<UserProfileDto?>(profile);
        }

        public async Task<UserProfileDto> CreateProfileAsync(CreateUserProfileDto createProfileDto)
        {
            var profile = _mapper.Map<UserProfile>(createProfileDto);
            await _repository.AddAsync(profile);
            return _mapper.Map<UserProfileDto>(profile);
        }

        public async Task UpdateProfileAsync(UserProfileDto profileDto)
        {
            var profile = await _repository.GetByIdAsync(profileDto.UserProfileId);
            if (profile == null) return;

            profile.FirstName = profileDto.FirstName;
            profile.LastName = profileDto.LastName;
            profile.Phone = profileDto.Phone;
            profile.Address = profileDto.Address;

            await _repository.UpdateAsync(profile);
        }

        public async Task DeleteProfileAsync(Guid profileId)
        {
            await _repository.DeleteAsync(profileId);
        }
    }
}
