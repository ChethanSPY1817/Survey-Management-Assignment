﻿namespace SurveyManagement.Application.DTOs.UserDTOs
{
    public class CreateUserDto
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string Password { get; set; } = string.Empty;

        // Add Role property
        public string Role { get; set; } = "Respondent"; // default
    }
}
