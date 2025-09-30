using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SurveyManagement.API.Controllers;
using SurveyManagement.Application.DTOs.UserDTOs;
using SurveyManagement.Application.Services;
using SurveyManagement.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SurveyManagement.Tests.Controllers
{
    [TestFixture]
    public class UserControllerTests
    {
        private Mock<IUserService> _userServiceMock = null!;
        private UserController _controller = null!;

        [SetUp]
        public void Setup()
        {
            _userServiceMock = new Mock<IUserService>();
            _controller = new UserController(_userServiceMock.Object);
        }

        private void SetUserClaims(Guid userId, string? role = null)
        {
            var claims = new List<Claim> { new Claim("UserId", userId.ToString()) };
            if (!string.IsNullOrEmpty(role)) claims.Add(new Claim(ClaimTypes.Role, role));

            var identity = new ClaimsIdentity(claims, "TestAuth");
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };
        }

        [Test]
        public async Task GetAll_ReturnsOkResult_WithUsers()
        {
            var users = new List<UserDto> { new UserDto { UserId = Guid.NewGuid(), Username = "TestUser" } };
            _userServiceMock.Setup(s => s.GetAllUsersAsync()).ReturnsAsync(users);

            SetUserClaims(Guid.NewGuid(), "Admin");
            var result = await _controller.GetAll() as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(users));
        }

        [Test]
        public void GetById_ThrowsUnauthorizedException_WhenUserIsNotAdminOrOwner()
        {
            var userId = Guid.NewGuid();
            _userServiceMock.Setup(s => s.GetUserByIdAsync(userId)).ReturnsAsync(new UserDto { UserId = userId });

            SetUserClaims(Guid.NewGuid(), "Respondent"); // Not owner or admin

            Assert.That(async () => await _controller.GetById(userId),
                        Throws.TypeOf<UnauthorizedException>());
        }

        [Test]
        public async Task Create_ReturnsCreatedAtActionResult()
        {
            var createDto = new CreateUserDto { Email = "test@test.com", Password = "Pass123" };
            var userDto = new UserDto { UserId = Guid.NewGuid(), Email = "test@test.com" };

            _userServiceMock.Setup(s => s.CreateUserAsync(createDto)).ReturnsAsync(userDto);
            SetUserClaims(Guid.NewGuid(), "Admin");

            var result = await _controller.Create(createDto) as CreatedAtActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.ActionName, Is.EqualTo(nameof(_controller.GetById)));

            var value = result.Value as UserDto;
            Assert.That(value, Is.Not.Null);
            Assert.That(value!.UserId, Is.EqualTo(userDto.UserId));
        }

        [Test]
        public async Task Register_ReturnsOkResult_WithUser()
        {
            var createDto = new CreateUserDto { Email = "test@test.com", Password = "Pass123" };
            var userDto = new UserDto
            {
                UserId = Guid.NewGuid(),
                Email = "test@test.com",
                Username = "test",
                Role = "Respondent"
            };

            _userServiceMock
                .Setup(s => s.CreateUserAsync(It.IsAny<CreateUserDto>()))
                .ReturnsAsync(userDto);

            var result = await _controller.Register(createDto) as OkObjectResult;

            Assert.That(result, Is.Not.Null);

            // Use JObject (or dynamic) to safely access properties of anonymous object
            var value = result!.Value;

            Assert.That(value, Is.Not.Null);

            // If using Newtonsoft.Json or System.Text.Json:
            var json = System.Text.Json.JsonSerializer.Serialize(value);
            var deserialized = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(json);

            Assert.That(Guid.Parse(deserialized!["UserId"].ToString()!), Is.EqualTo(userDto.UserId));
            Assert.That(deserialized["Role"].ToString(), Is.EqualTo("Respondent"));
        }

    }
}
