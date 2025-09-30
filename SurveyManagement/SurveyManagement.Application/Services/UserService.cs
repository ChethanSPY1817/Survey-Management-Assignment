using AutoMapper;
using BCrypt.Net;
using SurveyManagement.Application.DTOs.UserDTOs;
using SurveyManagement.Domain.Entities;
using SurveyManagement.Domain.Interfaces;
using SurveyManagement.Domain.Exceptions;

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
            if (users == null || !users.Any())
                throw new NotFoundException("User", Guid.Empty); // generic empty Id
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task<UserDto> GetUserByIdAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) throw new NotFoundException("User", userId);

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            if (createUserDto == null)
                throw new BadRequestException("CreateUserDto cannot be null.");

            var user = _mapper.Map<User>(createUserDto);

            if (!Enum.TryParse<UserRole>(createUserDto.Role, true, out var role))
                role = UserRole.Respondent;

            user.Role = role;
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password);

            await _userRepository.AddAsync(user);

            await _passwordHistoryRepository.AddAsync(new PasswordHistory
            {
                UserId = user.UserId,
                PlainPassword = createUserDto.Password
            });

            return _mapper.Map<UserDto>(user);
        }

        public async Task UpdateUserAsync(UserDto userDto)
        {
            var user = await _userRepository.GetByIdAsync(userDto.UserId);
            if (user == null) throw new NotFoundException("User", userDto.UserId);

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
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) throw new NotFoundException("User", userId);

            await _userRepository.DeleteAsync(userId);
        }
    }
}
