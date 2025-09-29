using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SurveyManagement.API.Controllers;
using SurveyManagement.Application.Services;
using SurveyManagement.Application.DTOs.SurveyDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SurveyManagement.Tests
{
    [TestFixture]
    public class SurveyControllerTests
    {
        private Mock<ISurveyService> _mockService = null!;
        private SurveyController _controller = null!;

        [SetUp]
        public void Setup()
        {
            _mockService = new Mock<ISurveyService>();
            _controller = new SurveyController(_mockService.Object);
        }

        [Test]
        public async Task GetAll_ReturnsOkResult_WithSurveys()
        {
            _mockService.Setup(s => s.GetAllAsync())
                .ReturnsAsync(new List<SurveyDto> { new SurveyDto { Title = "Survey A" } });

            var result = await _controller.GetAll();
            var okResult = result as OkObjectResult;
            var surveys = okResult?.Value as IEnumerable<SurveyDto>;

            Assert.That(okResult, Is.Not.Null);
            Assert.That(surveys?.Count() ?? 0, Is.EqualTo(1));
        }

        [Test]
        public async Task GetById_ReturnsOkResult_WhenSurveyExists()
        {
            var id = Guid.NewGuid();
            _mockService.Setup(s => s.GetByIdAsync(id))
                .ReturnsAsync(new SurveyDto { SurveyId = id, Title = "Survey A" });

            var result = await _controller.GetById(id);
            var okResult = result as OkObjectResult;
            var survey = okResult?.Value as SurveyDto;

            Assert.That(okResult, Is.Not.Null);
            Assert.That(survey?.SurveyId, Is.EqualTo(id));
        }

        [Test]
        public async Task GetById_ReturnsNotFound_WhenSurveyDoesNotExist()
        {
            var id = Guid.NewGuid();
            _mockService.Setup(s => s.GetByIdAsync(id)).ReturnsAsync((SurveyDto?)null);

            var result = await _controller.GetById(id);
            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public async Task Create_ReturnsOkResult()
        {
            var dto = new CreateSurveyDto { Title = "New Survey" };
            var result = await _controller.Create(dto);
            var okResult = result as OkObjectResult;

            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult?.Value, Is.EqualTo(dto));
        }

        [Test]
        public async Task Update_ReturnsNoContent_WhenSuccessful()
        {
            var id = Guid.NewGuid();
            var dto = new SurveyDto { SurveyId = id, Title = "Updated Survey" };

            _mockService.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(dto);

            var result = await _controller.Update(id, dto);
            Assert.That(result, Is.TypeOf<NoContentResult>());
        }

        [Test]
        public async Task Delete_ReturnsNoContent()
        {
            var id = Guid.NewGuid();
            var result = await _controller.Delete(id);
            Assert.That(result, Is.TypeOf<NoContentResult>());
        }
    }
}
 