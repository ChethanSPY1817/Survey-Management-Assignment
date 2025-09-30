using AutoMapper;
using BCrypt.Net;
using SurveyManagement.Application.DTOs.UserDTOs;
using SurveyManagement.Domain.Entities;
using SurveyManagement.Domain.Interfaces;
using SurveyManagement.Domain.Exceptions;
using SurveyManagement.CrossCutting.Logging;

namespace SurveyManagement.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPasswordHistoryRepository _passwordHistoryRepository;
        private readonly IServiceLogger<UserService> _logger;

        public UserService(
            IUserRepository userRepository,
            IMapper mapper,
            IPasswordHistoryRepository passwordHistoryRepository,
            IServiceLogger<UserService> logger)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _passwordHistoryRepository = passwordHistoryRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all users from database.");
                var users = await _userRepository.GetAllAsync();

                if (users == null || !users.Any())
                {
                    _logger.LogWarning("No users found in the database.");
                    throw new NotFoundException("User", Guid.Empty);
                }

                _logger.LogInformation($"Fetched {users.Count()} users successfully.");
                return _mapper.Map<IEnumerable<UserDto>>(users);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while fetching all users.", ex);
                throw;
            }
        }

        public async Task<UserDto> GetUserByIdAsync(Guid userId)
        {
            try
            {
                _logger.LogInformation($"Fetching user with ID: {userId}");
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning($"User with ID {userId} not found.");
                    throw new NotFoundException("User", userId);
                }

                _logger.LogInformation($"User with ID {userId} fetched successfully.");
                return _mapper.Map<UserDto>(user);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while fetching user with ID {userId}", ex);
                throw;
            }
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            try
            {
                _logger.LogInformation($"Creating a new user with email: {createUserDto?.Email}");
                if (createUserDto == null)
                {
                    _logger.LogWarning("CreateUserDto is null.");
                    throw new BadRequestException("CreateUserDto cannot be null.");
                }

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

                _logger.LogInformation($"User created successfully with ID: {user.UserId}");
                return _mapper.Map<UserDto>(user);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while creating a new user.", ex);
                throw;
            }
        }

        public async Task UpdateUserAsync(UserDto userDto)
        {
            try
            {
                _logger.LogInformation($"Updating user with ID: {userDto.UserId}");
                var user = await _userRepository.GetByIdAsync(userDto.UserId);
                if (user == null)
                {
                    _logger.LogWarning($"User with ID {userDto.UserId} not found.");
                    throw new NotFoundException("User", userDto.UserId);
                }

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
                _logger.LogInformation($"User with ID {userDto.UserId} updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while updating user with ID {userDto.UserId}", ex);
                throw;
            }
        }

        public async Task DeleteUserAsync(Guid userId)
        {
            try
            {
                _logger.LogInformation($"Deleting user with ID: {userId}");
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning($"User with ID {userId} not found.");
                    throw new NotFoundException("User", userId);
                }

                await _userRepository.DeleteAsync(userId);
                _logger.LogInformation($"User with ID {userId} deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while deleting user with ID {userId}", ex);
                throw;
            }
        }
    }
}
