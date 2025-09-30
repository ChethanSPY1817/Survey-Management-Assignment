using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SurveyManagement.API.Controllers;
using SurveyManagement.Application.DTOs.UserProfileDTOs;
using SurveyManagement.Application.Services;
using SurveyManagement.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SurveyManagement.Tests.Controllers
{
    [TestFixture]
    public class UserProfileControllerTests
    {
        private Mock<IUserProfileService> _serviceMock = null!;
        private UserProfileController _controller = null!;

        [SetUp]
        public void Setup()
        {
            _serviceMock = new Mock<IUserProfileService>();
            _controller = new UserProfileController(_serviceMock.Object);
        }

        private void SetUserClaims(Guid userId, string? role = null)
        {
            var claims = new List<Claim> { new Claim("UserId", userId.ToString()) };
            if (!string.IsNullOrEmpty(role)) claims.Add(new Claim(ClaimTypes.Role, role));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth")) }
            };
        }

        [Test]
        public async Task GetAll_ReturnsOkResult_WithProfiles()
        {
            var profiles = new List<UserProfileDto> { new() { UserProfileId = Guid.NewGuid() } };
            _serviceMock.Setup(s => s.GetAllProfilesAsync()).ReturnsAsync(profiles);

            SetUserClaims(Guid.NewGuid(), "Admin");
            var result = await _controller.GetAll() as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Value, Is.EqualTo(profiles));
        }

        [Test]
        public void GetById_ThrowsUnauthorizedException_WhenNotOwnerOrAdmin()
        {
            var profileId = Guid.NewGuid();
            var profile = new UserProfileDto { UserProfileId = profileId, UserId = Guid.NewGuid() };
            _serviceMock.Setup(s => s.GetProfileByIdAsync(profileId)).ReturnsAsync(profile);

            SetUserClaims(Guid.NewGuid(), "Respondent");

            Assert.That(async () => await _controller.GetById(profileId),
                        Throws.TypeOf<UnauthorizedException>());
        }

        [Test]
        public async Task Create_ReturnsCreatedAtActionResult()
        {
            var createDto = new CreateUserProfileDto { UserId = Guid.NewGuid() };
            var profileDto = new UserProfileDto { UserProfileId = Guid.NewGuid() };

            _serviceMock.Setup(s => s.CreateProfileAsync(createDto)).ReturnsAsync(profileDto);
            SetUserClaims(Guid.NewGuid(), "Admin");

            var result = await _controller.Create(createDto) as CreatedAtActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Value, Is.EqualTo(profileDto));
            Assert.That(result.ActionName, Is.EqualTo(nameof(_controller.GetById)));
        }

        [Test]
        public async Task Update_ReturnsNoContent_WhenAuthorized()
        {
            var userId = Guid.NewGuid();
            var profileDto = new UserProfileDto { UserProfileId = Guid.NewGuid(), UserId = userId };

            SetUserClaims(userId);
            _serviceMock.Setup(s => s.UpdateProfileAsync(profileDto)).Returns(Task.CompletedTask);

            var result = await _controller.Update(profileDto.UserProfileId, profileDto) as NoContentResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(204));
        }
    }
}
