using AutoMapper;
using Moq;
using NUnit.Framework;
using SurveyManagement.Application.DTOs.UserSurveyDTOs;
using SurveyManagement.Application.Services;
using SurveyManagement.Domain.Entities;
using SurveyManagement.Domain.Exceptions;
using SurveyManagement.Domain.Interfaces;
using SurveyManagement.CrossCutting.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SurveyManagement.Tests.Services
{
    [TestFixture]
    public class UserSurveyServiceTests
    {
        private Mock<IUserSurveyRepository> _repoMock = null!;
        private Mock<IMapper> _mapperMock = null!;
        private Mock<IServiceLogger<UserSurveyService>> _loggerMock = null!;
        private UserSurveyService _service = null!;

        [SetUp]
        public void Setup()
        {
            _repoMock = new Mock<IUserSurveyRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<IServiceLogger<UserSurveyService>>();
            _service = new UserSurveyService(_repoMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task GetUserSurveysByCreatorIdAsync_ReturnsMappedDtos()
        {
            var creatorId = Guid.NewGuid();
            var entities = new List<UserSurvey> { new UserSurvey { UserSurveyId = Guid.NewGuid() } };
            _repoMock.Setup(r => r.GetByCreatorIdAsync(creatorId)).ReturnsAsync(entities);
            _mapperMock.Setup(m => m.Map<IEnumerable<UserSurveyDto>>(entities))
                       .Returns(new List<UserSurveyDto> { new UserSurveyDto { UserSurveyId = entities[0].UserSurveyId } });

            var result = await _service.GetUserSurveysByCreatorIdAsync(creatorId);

            Assert.That(result, Has.Exactly(1).Items);
        }

        [Test]
        public async Task GetUserSurveyByIdAsync_ReturnsDto_WhenExists()
        {
            var id = Guid.NewGuid();
            var entity = new UserSurvey { UserSurveyId = id };
            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);
            _mapperMock.Setup(m => m.Map<UserSurveyDto>(entity)).Returns(new UserSurveyDto { UserSurveyId = id });

            var result = await _service.GetUserSurveyByIdAsync(id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.UserSurveyId, Is.EqualTo(id));
        }

        //[Test]
        //public async Task CreateUserSurveyAsync_CreatesSuccessfully()
        //{
        //    var dto = new CreateUserSurveyDto { Title = "Test" };
        //    var entity = new UserSurvey { UserSurveyId = Guid.NewGuid() };
        //    _mapperMock.Setup(m => m.Map<UserSurvey>(dto)).Returns(entity);
        //    _repoMock.Setup(r => r.AddAsync(entity)).Returns(Task.CompletedTask);
        //    _mapperMock.Setup(m => m.Map<UserSurveyDto>(entity)).Returns(new UserSurveyDto { UserSurveyId = entity.UserSurveyId });

        //    var result = await _service.CreateUserSurveyAsync(dto, Guid.NewGuid());

        //    Assert.That(result, Is.Not.Null);
        //    Assert.That(result.UserSurveyId, Is.EqualTo(entity.UserSurveyId));
        //}

        [Test]
        public async Task UpdateUserSurveyAsync_UpdatesSuccessfully()
        {
            var id = Guid.NewGuid();
            var dto = new UserSurveyDto { UserSurveyId = id };
            var entity = new UserSurvey { UserSurveyId = id };

            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);
            _repoMock.Setup(r => r.UpdateAsync(entity)).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map(dto, entity));

            await _service.UpdateUserSurveyAsync(dto);

            _repoMock.Verify(r => r.UpdateAsync(entity), Times.Once);
        }

        [Test]
        public void UpdateUserSurveyAsync_ThrowsNotFoundException_WhenNotFound()
        {
            var dto = new UserSurveyDto { UserSurveyId = Guid.NewGuid() };
            _repoMock.Setup(r => r.GetByIdAsync(dto.UserSurveyId)).ReturnsAsync((UserSurvey?)null);

            Assert.That(async () => await _service.UpdateUserSurveyAsync(dto), Throws.TypeOf<NotFoundException>());
        }

        [Test]
        public async Task DeleteUserSurveyAsync_DeletesSuccessfully()
        {
            var id = Guid.NewGuid();
            var entity = new UserSurvey { UserSurveyId = id };
            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);
            _repoMock.Setup(r => r.DeleteAsync(id)).Returns(Task.CompletedTask);

            await _service.DeleteUserSurveyAsync(id);

            _repoMock.Verify(r => r.DeleteAsync(id), Times.Once);
        }

        [Test]
        public void DeleteUserSurveyAsync_ThrowsNotFoundException_WhenNotFound()
        {
            var id = Guid.NewGuid();
            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((UserSurvey?)null);

            Assert.That(async () => await _service.DeleteUserSurveyAsync(id), Throws.TypeOf<NotFoundException>());
        }
    }
}
