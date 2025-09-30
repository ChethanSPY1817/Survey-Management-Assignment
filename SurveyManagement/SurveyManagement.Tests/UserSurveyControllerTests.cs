using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SurveyManagement.API.Controllers;
using SurveyManagement.Application.DTOs.UserSurveyDTOs;
using SurveyManagement.Application.Services;
using SurveyManagement.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SurveyManagement.Tests.Controllers
{
    [TestFixture]
    public class UserSurveyControllerTests
    {
        private Mock<IUserSurveyService> _serviceMock = null!;
        private UserSurveyController _controller = null!;

        [SetUp]
        public void Setup()
        {
            _serviceMock = new Mock<IUserSurveyService>();
            _controller = new UserSurveyController(_serviceMock.Object);

            // Mock HttpContext to provide a logged-in user
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Test]
        public async Task GetAll_ReturnsOk_WithUserSurveys()
        {
            var surveys = new List<UserSurveyDto> { new UserSurveyDto { UserSurveyId = Guid.NewGuid() } };
            _serviceMock.Setup(s => s.GetUserSurveysByCreatorIdAsync(It.IsAny<Guid>()))
                        .ReturnsAsync(surveys);

            var result = await _controller.GetAll() as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Value, Is.EqualTo(surveys));
        }

        [Test]
        public async Task GetById_ReturnsOk_WhenSurveyExists()
        {
            var surveyId = Guid.NewGuid();
            var survey = new UserSurveyDto { UserSurveyId = surveyId, CreatedById = Guid.Parse(_controller.User.FindFirstValue(ClaimTypes.NameIdentifier)!) };
            _serviceMock.Setup(s => s.GetUserSurveyByIdAsync(surveyId)).ReturnsAsync(survey);

            var result = await _controller.GetById(surveyId) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Value, Is.EqualTo(survey));
        }

        [Test]
        public void GetById_ThrowsNotFoundException_WhenSurveyNotFound()
        {
            var surveyId = Guid.NewGuid();
            _serviceMock.Setup(s => s.GetUserSurveyByIdAsync(surveyId)).ReturnsAsync((UserSurveyDto?)null);

            Assert.That(async () => await _controller.GetById(surveyId),
                        Throws.TypeOf<NotFoundException>());
        }

        [Test]
        public async Task Create_ReturnsCreatedAtAction_WhenSuccessful()
        {
            var dto = new CreateUserSurveyDto { Title = "Test" };
            var createdSurvey = new UserSurveyDto { UserSurveyId = Guid.NewGuid() };
            _serviceMock.Setup(s => s.CreateUserSurveyAsync(dto, It.IsAny<Guid>()))
                        .ReturnsAsync(createdSurvey);

            var result = await _controller.Create(dto) as CreatedAtActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Value, Is.EqualTo(createdSurvey));
        }

        [Test]
        public void Create_ThrowsBadRequestException_WhenDtoIsNull()
        {
            Assert.That(async () => await _controller.Create(null!), Throws.TypeOf<BadRequestException>());
        }

        [Test]
        public async Task Update_ReturnsNoContent_WhenSuccessful()
        {
            var userId = Guid.Parse(_controller.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var surveyId = Guid.NewGuid();
            var dto = new UserSurveyDto { UserSurveyId = surveyId, CreatedById = userId };

            _serviceMock.Setup(s => s.GetUserSurveyByIdAsync(surveyId)).ReturnsAsync(dto);
            _serviceMock.Setup(s => s.UpdateUserSurveyAsync(dto)).Returns(Task.CompletedTask);

            var result = await _controller.Update(surveyId, dto) as NoContentResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(204));
        }

        [Test]
        public void Update_ThrowsBadRequestException_WhenIdMismatch()
        {
            var dto = new UserSurveyDto { UserSurveyId = Guid.NewGuid() };
            Assert.That(async () => await _controller.Update(Guid.NewGuid(), dto), Throws.TypeOf<BadRequestException>());
        }

        [Test]
        public async Task Delete_ReturnsNoContent_WhenSuccessful()
        {
            var userId = Guid.Parse(_controller.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var surveyId = Guid.NewGuid();
            var survey = new UserSurveyDto { UserSurveyId = surveyId, CreatedById = userId };

            _serviceMock.Setup(s => s.GetUserSurveyByIdAsync(surveyId)).ReturnsAsync(survey);
            _serviceMock.Setup(s => s.DeleteUserSurveyAsync(surveyId)).Returns(Task.CompletedTask);

            var result = await _controller.Delete(surveyId) as NoContentResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(204));
        }
    }
}
