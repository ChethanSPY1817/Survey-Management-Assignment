using AutoMapper;
using BCrypt.Net;
using SurveyManagement.Application.DTOs;
using SurveyManagement.Application.DTOs.UserDTOs;
using SurveyManagement.Domain.Entities;
using SurveyManagement.Domain.Interfaces;
using SurveyManagement.Infrastructure.Repositories;

namespace SurveyManagement.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        private readonly IPasswordHistoryRepository _passwordHistoryRepository;

        public UserService(IUserRepository userRepository, IMapper mapper, IPasswordHistoryRepository passwordHistoryRepository)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _passwordHistoryRepository = passwordHistoryRepository;
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

            // Assign role dynamically
            if (!Enum.TryParse<UserRole>(createUserDto.Role, true, out var role))
                role = UserRole.Respondent; // fallback default

            user.Role = role;

            // Hash password for User table
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password);

            await _userRepository.AddAsync(user);

            // Store plain password in PasswordHistory
            var history = new PasswordHistory
            {
                UserId = user.UserId,
                PlainPassword = createUserDto.Password
            };
            await _passwordHistoryRepository.AddAsync(history);

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
