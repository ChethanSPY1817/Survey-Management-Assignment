using AutoMapper;
using Moq;
using NUnit.Framework;
using SurveyManagement.Application.DTOs.UserDTOs;
using SurveyManagement.Application.Services;
using SurveyManagement.Domain.Entities;
using SurveyManagement.Domain.Exceptions;
using SurveyManagement.Domain.Interfaces;
using SurveyManagement.CrossCutting.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SurveyManagement.Tests.Services
{
    [TestFixture]
    public class UserServiceTests
    {
        private Mock<IUserRepository> _userRepositoryMock = null!;
        private Mock<IPasswordHistoryRepository> _passwordHistoryMock = null!;
        private Mock<IMapper> _mapperMock = null!;
        private Mock<IServiceLogger<UserService>> _loggerMock = null!;
        private UserService _userService = null!;

        [SetUp]
        public void Setup()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _passwordHistoryMock = new Mock<IPasswordHistoryRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<IServiceLogger<UserService>>();

            _userService = new UserService(
                _userRepositoryMock.Object,
                _mapperMock.Object,
                _passwordHistoryMock.Object,
                _loggerMock.Object
            );
        }

        [Test]
        public async Task GetAllUsersAsync_ReturnsUserDtoList()
        {
            var users = new List<User> { new User { UserId = Guid.NewGuid(), Username = "TestUser" } };
            _userRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(users);
            _mapperMock.Setup(m => m.Map<IEnumerable<UserDto>>(users))
                       .Returns(new List<UserDto> { new UserDto { Username = "TestUser" } });

            var result = await _userService.GetAllUsersAsync();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(1));
        }

        [Test]
        public void GetAllUsersAsync_ThrowsNotFoundException_WhenNoUsers()
        {
            _userRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<User>());

            Assert.That(async () => await _userService.GetAllUsersAsync(),
                        Throws.TypeOf<NotFoundException>());
        }

        [Test]
        public async Task GetUserByIdAsync_ReturnsUserDto()
        {
            var userId = Guid.NewGuid();
            var user = new User { UserId = userId, Username = "Test" };
            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
            _mapperMock.Setup(m => m.Map<UserDto>(user))
                       .Returns(new UserDto { UserId = userId, Username = "Test" });

            var result = await _userService.GetUserByIdAsync(userId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.UserId, Is.EqualTo(userId));
        }

        [Test]
        public void GetUserByIdAsync_ThrowsNotFoundException_WhenUserNotFound()
        {
            var userId = Guid.NewGuid();
            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((User?)null);

            Assert.That(async () => await _userService.GetUserByIdAsync(userId),
                        Throws.TypeOf<NotFoundException>());
        }

        [Test]
        public async Task CreateUserAsync_CreatesUserSuccessfully()
        {
            var createDto = new CreateUserDto { Email = "test@example.com", Password = "Password123", Role = "Admin" };
            var user = new User { UserId = Guid.NewGuid(), Email = createDto.Email };

            _mapperMock.Setup(m => m.Map<User>(createDto)).Returns(user);
            _userRepositoryMock.Setup(r => r.AddAsync(user)).Returns(Task.CompletedTask);
            _passwordHistoryMock.Setup(p => p.AddAsync(It.IsAny<PasswordHistory>())).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<UserDto>(user)).Returns(new UserDto { UserId = user.UserId, Email = user.Email });

            var result = await _userService.CreateUserAsync(createDto);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.UserId, Is.EqualTo(user.UserId));
        }

        [Test]
        public void CreateUserAsync_ThrowsBadRequestException_WhenDtoIsNull()
        {
            Assert.That(async () => await _userService.CreateUserAsync(null),
                        Throws.TypeOf<BadRequestException>());
        }

        [Test]
        public async Task UpdateUserAsync_UpdatesUserSuccessfully()
        {
            var userDto = new UserDto { UserId = Guid.NewGuid(), Username = "Updated", Email = "update@example.com" };
            var user = new User { UserId = userDto.UserId, Username = "Old", Email = "old@example.com" };

            _userRepositoryMock.Setup(r => r.GetByIdAsync(userDto.UserId)).ReturnsAsync(user);
            _userRepositoryMock.Setup(r => r.UpdateAsync(user)).Returns(Task.CompletedTask);

            Assert.That(async () => await _userService.UpdateUserAsync(userDto), Throws.Nothing);
        }

        [Test]
        public void UpdateUserAsync_ThrowsNotFoundException_WhenUserNotFound()
        {
            var userDto = new UserDto { UserId = Guid.NewGuid() };
            _userRepositoryMock.Setup(r => r.GetByIdAsync(userDto.UserId)).ReturnsAsync((User?)null);

            Assert.That(async () => await _userService.UpdateUserAsync(userDto),
                        Throws.TypeOf<NotFoundException>());
        }

        [Test]
        public async Task DeleteUserAsync_DeletesUserSuccessfully()
        {
            var userId = Guid.NewGuid();
            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(new User { UserId = userId });
            _userRepositoryMock.Setup(r => r.DeleteAsync(userId)).Returns(Task.CompletedTask);

            Assert.That(async () => await _userService.DeleteUserAsync(userId), Throws.Nothing);
        }

        [Test]
        public void DeleteUserAsync_ThrowsNotFoundException_WhenUserNotFound()
        {
            var userId = Guid.NewGuid();
            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((User?)null);

            Assert.That(async () => await _userService.DeleteUserAsync(userId),
                        Throws.TypeOf<NotFoundException>());
        }
    }
}
