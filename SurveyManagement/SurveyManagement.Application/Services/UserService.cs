using SurveyManagement.Application.DTOs.UserDTOs;
using SurveyManagement.Application.DTOs;
using SurveyManagement.Domain.Entities;
using SurveyManagement.Domain.Interfaces;
using AutoMapper;

namespace SurveyManagement.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task<UserDto?> GetUserByIdAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return null;
            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            var user = _mapper.Map<User>(createUserDto);
            await _userRepository.AddAsync(user);
            return _mapper.Map<UserDto>(user);
        }

        public async Task UpdateUserAsync(UserDto userDto)
        {
            var user = await _userRepository.GetByIdAsync(userDto.UserId);
            if (user == null) return;

            user.Username = userDto.Username;
            user.Email = userDto.Email;

            if (user.Profile != null)
            {
                user.Profile.FirstName = userDto.FirstName;
                user.Profile.LastName = userDto.LastName;
                user.Profile.Phone = userDto.Phone;
                user.Profile.Address = userDto.Address;
            }

            await _userRepository.UpdateAsync(user);
        }

        public async Task DeleteUserAsync(Guid userId)
        {
            await _userRepository.DeleteAsync(userId);
        }
    }
}
