using NUnit.Framework;
using Moq;
using SurveyManagement.API.Controllers;
using SurveyManagement.Application.Services;
using SurveyManagement.Application.DTOs.UserProfileDTOs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SurveyManagement.Tests
{
    [TestFixture]
    public class UserProfileControllerTests
    {
        private Mock<IUserProfileService> _mockService = null!;
        private UserProfileController _controller = null!;

        [SetUp]
        public void Setup()
        {
            _mockService = new Mock<IUserProfileService>();
            _controller = new UserProfileController(_mockService.Object);
        }

        [Test]
        public async Task GetAll_ReturnsOkResult_WithProfiles()
        {
            // Arrange
            _mockService.Setup(s => s.GetAllProfilesAsync())
                .ReturnsAsync(new List<UserProfileDto>
                {
                    new UserProfileDto { UserProfileId = Guid.NewGuid(), FirstName = "John", LastName = "Doe" }
                });

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);

            var profiles = okResult?.Value as IEnumerable<UserProfileDto>;
            Assert.That(profiles, Is.Not.Null);
            Assert.That(profiles?.Count() ?? 0, Is.EqualTo(1));
        }

        [Test]
        public async Task GetById_ReturnsNotFound_WhenProfileDoesNotExist()
        {
            // Arrange
            var id = Guid.NewGuid();
            _mockService.Setup(s => s.GetProfileByIdAsync(id))
                .ReturnsAsync((UserProfileDto?)null);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public async Task Create_ReturnsCreatedAtAction_WithProfile()
        {
            // Arrange
            var dto = new CreateUserProfileDto
            {
                UserId = Guid.NewGuid(),
                FirstName = "Jane",
                LastName = "Doe"
            };

            var returnedDto = new UserProfileDto
            {
                UserProfileId = Guid.NewGuid(),
                UserId = dto.UserId,
                FirstName = dto.FirstName,
                LastName = dto.LastName
            };

            _mockService.Setup(s => s.CreateProfileAsync(dto))
                .ReturnsAsync(returnedDto);

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var createdResult = result as CreatedAtActionResult;
            Assert.That(createdResult, Is.Not.Null);
            Assert.That(createdResult?.Value, Is.EqualTo(returnedDto));
        }

        [Test]
        public async Task Delete_ReturnsNoContent_WhenProfileExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var profile = new UserProfileDto { UserProfileId = id, FirstName = "John" };

            _mockService.Setup(s => s.GetProfileByIdAsync(id)).ReturnsAsync(profile);
            _mockService.Setup(s => s.DeleteProfileAsync(id)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(id);

            // Assert
            Assert.That(result, Is.TypeOf<NoContentResult>());
        }
    }
}
