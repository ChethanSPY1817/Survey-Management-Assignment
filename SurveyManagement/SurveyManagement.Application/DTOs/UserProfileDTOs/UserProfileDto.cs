namespace SurveyManagement.Application.DTOs.UserProfileDTOs
{
    public class UserProfileDto
    {
        public Guid UserProfileId { get; set; }
        public Guid UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Address { get; set; }
    }
}
