using BCrypt.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SurveyManagement.Application.DTOs.AuthDTOs;
using SurveyManagement.Domain.Entities;
using SurveyManagement.Domain.Exceptions;
using SurveyManagement.Domain.Interfaces;
using SurveyManagement.CrossCutting.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SurveyManagement.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IServiceLogger<AuthService> _logger;

        public AuthService(
            IUserRepository userRepository,
            IConfiguration configuration,
            IServiceLogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            try
            {
                _logger.LogInformation($"Attempting login for email: {loginDto.Email}");

                var users = await _userRepository.GetAllAsync();
                var user = users.FirstOrDefault(u => u.Email == loginDto.Email);

                if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                {
                    _logger.LogWarning($"Login failed for email: {loginDto.Email}");
                    throw new UnauthorizedAccessException("Invalid email or password.");
                }

                _logger.LogInformation($"Login successful for email: {loginDto.Email}");
                return GenerateJwt(user);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception occurred during login for email: {loginDto.Email}", ex);
                throw;
            }
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            try
            {
                _logger.LogInformation($"Attempting registration for email: {registerDto.Email}");

                var users = await _userRepository.GetAllAsync();
                if (users.Any(u => u.Email == registerDto.Email))
                {
                    _logger.LogWarning($"Registration failed: email already exists - {registerDto.Email}");
                    throw new BadRequestException("Email is already registered.");
                }

                var newUser = new User
                {
                    UserId = Guid.NewGuid(),
                    Username = registerDto.Username,
                    Email = registerDto.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                    Role = Enum.Parse<UserRole>(registerDto.Role, ignoreCase: true)
                };

                await _userRepository.AddAsync(newUser);

                _logger.LogInformation($"User registered successfully with ID: {newUser.UserId}");
                return GenerateJwt(newUser);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception occurred during registration for email: {registerDto.Email}", ex);
                throw;
            }
        }

        private AuthResponseDto GenerateJwt(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!);
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var expiryMinutes = int.Parse(jwtSettings["ExpiryMinutes"] ?? "60");

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(expiryMinutes),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            _logger.LogInformation($"JWT generated for user ID: {user.UserId}");

            return new AuthResponseDto
            {
                Token = tokenHandler.WriteToken(token),
                Role = user.Role.ToString(),
                Username = user.Username
            };
        }
    }
}
