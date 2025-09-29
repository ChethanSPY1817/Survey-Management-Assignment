using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SurveyManagement.Application.DTOs.AuthDTOs;
using SurveyManagement.Domain.Entities;
using SurveyManagement.Domain.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SurveyManagement.Application.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
    }

}
