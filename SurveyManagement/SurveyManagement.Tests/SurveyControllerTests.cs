using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SurveyManagement.API.Controllers;
using SurveyManagement.Application.DTOs.SurveyDTOs;
using SurveyManagement.Application.Services;
using SurveyManagement.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SurveyManagement.Tests.Controllers
{
    [TestFixture]
    public class SurveyControllerTests
    {
        private Mock<ISurveyService> _serviceMock = null!;
        private SurveyController _controller = null!;

        [SetUp]
        public void Setup()
        {
            _serviceMock = new Mock<ISurveyService>();
            _controller = new SurveyController(_serviceMock.Object);
        }

        private void SetUserClaims(Guid userId, string? role = null)
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            if (!string.IsNullOrEmpty(role)) claims.Add(new Claim(ClaimTypes.Role, role));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth")) }
            };
        }

        [Test]
        public async Task GetAll_ReturnsOkResult_WithSurveys()
        {
            var surveys = new List<SurveyDto> { new() { SurveyId = Guid.NewGuid() } };
            _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(surveys);

            var result = await _controller.GetAll() as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Value, Is.EqualTo(surveys));
        }

        [Test]
        public async Task GetById_ReturnsOkResult()
        {
            var surveyId = Guid.NewGuid();
            var survey = new SurveyDto { SurveyId = surveyId };
            _serviceMock.Setup(s => s.GetByIdAsync(surveyId)).ReturnsAsync(survey);

            var result = await _controller.GetById(surveyId) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Value, Is.EqualTo(survey));
        }

        [Test]
        public async Task Create_ReturnsOkResult_WithCreatedSurvey()
        {
            var userId = Guid.NewGuid();
            var createDto = new CreateSurveyDto { Title = "Survey" };
            var surveyDto = new SurveyDto { SurveyId = Guid.NewGuid(), Title = "Survey" };

            _serviceMock.Setup(s => s.CreateAsync(createDto, userId)).ReturnsAsync(surveyDto);
            SetUserClaims(userId, "Admin");

            var result = await _controller.Create(createDto) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            var value = result!.Value;

            // Convert to JSON then Dictionary to access properties
            var json = System.Text.Json.JsonSerializer.Serialize(value);
            var deserialized = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(json);

            Assert.That(Guid.Parse(deserialized!["SurveyId"].ToString()!), Is.EqualTo(surveyDto.SurveyId));
            Assert.That(deserialized["Title"].ToString(), Is.EqualTo("Survey"));
        }


        [Test]
        public void Update_ThrowsBadRequestException_WhenIdMismatch()
        {
            var dto = new SurveyDto { SurveyId = Guid.NewGuid() };
            SetUserClaims(Guid.NewGuid(), "Admin");

            Assert.That(async () => await _controller.Update(Guid.NewGuid(), dto),
                        Throws.TypeOf<BadRequestException>());
        }
    }
}
