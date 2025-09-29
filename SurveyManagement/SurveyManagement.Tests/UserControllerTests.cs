using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SurveyManagement.API.Controllers;
using SurveyManagement.Application.DTOs.UserDTOs;
using SurveyManagement.Application.Services;

namespace SurveyManagement.Tests
{
    [TestFixture]
    public class UserControllerTests
    {
        private Mock<IUserService> _mockService = null!;
        private UserController _controller = null!;

        [SetUp]
        public void Setup()
        {
            _mockService = new Mock<IUserService>();
            _controller = new UserController(_mockService.Object);
        }

        [Test]
        public async Task GetAll_ReturnsOkResult_WithUsers()
        {
            _mockService.Setup(s => s.GetAllUsersAsync())
                .ReturnsAsync(new List<UserDto> { new UserDto { Username = "test" } });

            var result = await _controller.GetAll();
            var okResult = result as OkObjectResult;
            var users = okResult?.Value as IEnumerable<UserDto>;

            Assert.That(okResult, Is.Not.Null);
            Assert.That(users?.Count() ?? 0, Is.EqualTo(1));
        }

        [Test]
        public async Task GetById_ReturnsOkResult_WhenUserExists()
        {
            var id = Guid.NewGuid();
            _mockService.Setup(s => s.GetUserByIdAsync(id))
                .ReturnsAsync(new UserDto { UserId = id, Username = "test" });

            var result = await _controller.GetById(id);
            var okResult = result as OkObjectResult;
            var user = okResult?.Value as UserDto;

            Assert.That(okResult, Is.Not.Null);
            Assert.That(user?.UserId, Is.EqualTo(id));
        }

        [Test]
        public async Task GetById_ReturnsNotFound_WhenUserDoesNotExist()
        {
            var id = Guid.NewGuid();
            _mockService.Setup(s => s.GetUserByIdAsync(id))
                .ReturnsAsync((UserDto?)null);

            var result = await _controller.GetById(id);
            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public async Task Create_ReturnsCreatedAtAction()
        {
            var createUserDto = new CreateUserDto
            {
                Username = "newuser",
                Email = "newuser@example.com",
                FirstName = "New",
                LastName = "User",
                Phone = "1234567890",
                Address = "Some Address"
            };

            var result = await _controller.Create(createUserDto);
            var createdResult = result as CreatedAtActionResult;

            Assert.That(createdResult, Is.Not.Null);
            Assert.That(createdResult?.Value, Is.EqualTo(createUserDto));
        }

        [Test]
        public async Task Update_ReturnsNoContent_WhenSuccessful()
        {
            var id = Guid.NewGuid();
            var userDto = new UserDto { UserId = id, Username = "updateduser" };

            _mockService.Setup(s => s.GetUserByIdAsync(id)).ReturnsAsync(userDto);

            var result = await _controller.Update(id, userDto);
            Assert.That(result, Is.TypeOf<NoContentResult>());
        }

        [Test]
        public async Task Update_ReturnsBadRequest_WhenIdMismatch()
        {
            var userDto = new UserDto { UserId = Guid.NewGuid(), Username = "updateduser" };
            var result = await _controller.Update(Guid.NewGuid(), userDto);
            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task Delete_ReturnsNoContent_WhenSuccessful()
        {
            var id = Guid.NewGuid();
            var userDto = new UserDto { UserId = id, Username = "userToDelete" };
            _mockService.Setup(s => s.GetUserByIdAsync(id)).ReturnsAsync(userDto);

            var result = await _controller.Delete(id);
            Assert.That(result, Is.TypeOf<NoContentResult>());
        }

        [Test]
        public async Task Delete_ReturnsNotFound_WhenUserDoesNotExist()
        {
            var id = Guid.NewGuid();
            _mockService.Setup(s => s.GetUserByIdAsync(id)).ReturnsAsync((UserDto?)null);

            var result = await _controller.Delete(id);
            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }
    }
}
