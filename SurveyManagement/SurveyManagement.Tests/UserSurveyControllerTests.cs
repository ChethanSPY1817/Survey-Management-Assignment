using NUnit.Framework;
using Moq;
using Microsoft.AspNetCore.Mvc;
using SurveyManagement.API.Controllers;
using SurveyManagement.Application.Services;
using SurveyManagement.Application.DTOs.UserSurveyDTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace SurveyManagement.Tests
{
    [TestFixture]
    public class UserSurveyControllerTests
    {
        private Mock<IUserSurveyService> _mockService = null!;
        private UserSurveyController _controller = null!;

        [SetUp]
        public void Setup()
        {
            _mockService = new Mock<IUserSurveyService>();
            _controller = new UserSurveyController(_mockService.Object);
        }

        [Test]
        public async Task GetAll_ReturnsOkResult_WithUserSurveys()
        {
            var list = new List<UserSurveyDto> { new UserSurveyDto { UserSurveyId = Guid.NewGuid() } };
            _mockService.Setup(s => s.GetAllUserSurveysAsync()).ReturnsAsync(list);

            var result = await _controller.GetAll();
            var okResult = result as OkObjectResult;
            var surveys = okResult?.Value as IEnumerable<UserSurveyDto>;

            Assert.That(okResult, Is.Not.Null);
            Assert.That(surveys?.Count() ?? 0, Is.EqualTo(1));
        }

        [Test]
        public async Task GetById_ReturnsOkResult_WhenExists()
        {
            var id = Guid.NewGuid();
            var survey = new UserSurveyDto { UserSurveyId = id };
            _mockService.Setup(s => s.GetUserSurveyByIdAsync(id)).ReturnsAsync(survey);

            var result = await _controller.GetById(id);
            var okResult = result as OkObjectResult;
            var returnedSurvey = okResult?.Value as UserSurveyDto;

            Assert.That(okResult, Is.Not.Null);
            Assert.That(returnedSurvey?.UserSurveyId, Is.EqualTo(id));
        }

        [Test]
        public async Task GetById_ReturnsNotFound_WhenDoesNotExist()
        {
            var id = Guid.NewGuid();
            _mockService.Setup(s => s.GetUserSurveyByIdAsync(id)).ReturnsAsync((UserSurveyDto?)null);

            var result = await _controller.GetById(id);
            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public async Task Create_ReturnsCreatedAtAction()
        {
            var createDto = new CreateUserSurveyDto { UserId = Guid.NewGuid(), SurveyId = Guid.NewGuid() };
            var created = new UserSurveyDto { UserSurveyId = Guid.NewGuid() };
            //_mockService.Setup(s => s.CreateUserSurveyAsync(createDto)).ReturnsAsync(created);

            var result = await _controller.Create(createDto);
            var createdResult = result as CreatedAtActionResult;
            var returnedSurvey = createdResult?.Value as UserSurveyDto;

            Assert.That(createdResult, Is.Not.Null);
            Assert.That(returnedSurvey?.UserSurveyId, Is.EqualTo(created.UserSurveyId));
        }

        [Test]
        public async Task Update_ReturnsNoContent_WhenSuccessful()
        {
            var id = Guid.NewGuid();
            var dto = new UserSurveyDto { UserSurveyId = id };
            _mockService.Setup(s => s.UpdateUserSurveyAsync(dto)).Returns(Task.CompletedTask);

            var result = await _controller.Update(id, dto);
            Assert.That(result, Is.TypeOf<NoContentResult>());
        }

        [Test]
        public async Task Update_ReturnsBadRequest_WhenIdMismatch()
        {
            var dto = new UserSurveyDto { UserSurveyId = Guid.NewGuid() };
            var result = await _controller.Update(Guid.NewGuid(), dto);
            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task Delete_ReturnsNoContent()
        {
            var id = Guid.NewGuid();
            _mockService.Setup(s => s.DeleteUserSurveyAsync(id)).Returns(Task.CompletedTask);

            var result = await _controller.Delete(id);
            Assert.That(result, Is.TypeOf<NoContentResult>());
        }
    }
}
