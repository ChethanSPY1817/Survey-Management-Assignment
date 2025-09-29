using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SurveyManagement.API.Controllers;
using SurveyManagement.Application.DTOs.ResponseDTOs;
using SurveyManagement.Application.Services;

namespace SurveyManagement.Tests
{
    [TestFixture]
    public class ResponseControllerTests
    {
        private Mock<IResponseService> _mockService = null!;
        private ResponseController _controller = null!;

        [SetUp]
        public void Setup()
        {
            _mockService = new Mock<IResponseService>();
            _controller = new ResponseController(_mockService.Object);
        }

        [Test]
        public async Task GetAll_ReturnsOkResult_WithResponses()
        {
            _mockService.Setup(s => s.GetAllResponsesAsync())
                .ReturnsAsync(new List<ResponseDto> { new ResponseDto { TextAnswer = "Answer1" } });

            var result = await _controller.GetAll();
            var okResult = result as OkObjectResult;
            var responses = okResult?.Value as IEnumerable<ResponseDto>;

            Assert.That(okResult, Is.Not.Null);
            Assert.That(responses?.Count() ?? 0, Is.EqualTo(1));
        }

        [Test]
        public async Task GetById_ReturnsOkResult_WhenResponseExists()
        {
            var id = Guid.NewGuid();
            _mockService.Setup(s => s.GetResponseByIdAsync(id))
                .ReturnsAsync(new ResponseDto { ResponseId = id, TextAnswer = "Answer1" });

            var result = await _controller.GetById(id);
            var okResult = result as OkObjectResult;
            var response = okResult?.Value as ResponseDto;

            Assert.That(okResult, Is.Not.Null);
            Assert.That(response?.ResponseId, Is.EqualTo(id));
        }

        [Test]
        public async Task GetById_ReturnsNotFound_WhenResponseDoesNotExist()
        {
            var id = Guid.NewGuid();
            _mockService.Setup(s => s.GetResponseByIdAsync(id))
                .ReturnsAsync((ResponseDto?)null);

            var result = await _controller.GetById(id);
            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public async Task Create_ReturnsOkResult()
        {
            var createDto = new CreateResponseDto
            {
                UserSurveyId = Guid.NewGuid(),
                QuestionId = Guid.NewGuid(),
                TextAnswer = "Test Answer"
            };

            var result = await _controller.Create(createDto);
            var okResult = result as OkObjectResult;

            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult?.Value, Is.EqualTo(createDto));
        }

        [Test]
        public async Task Update_ReturnsNoContent_WhenSuccessful()
        {
            var id = Guid.NewGuid();
            var responseDto = new ResponseDto
            {
                ResponseId = id,
                TextAnswer = "Updated Answer"
            };

            _mockService.Setup(s => s.GetResponseByIdAsync(id)).ReturnsAsync(responseDto);

            var result = await _controller.Update(id, responseDto);
            Assert.That(result, Is.TypeOf<NoContentResult>());
        }

        [Test]
        public async Task Update_ReturnsBadRequest_WhenIdMismatch()
        {
            var responseDto = new ResponseDto
            {
                ResponseId = Guid.NewGuid(),
                TextAnswer = "Updated Answer"
            };

            var result = await _controller.Update(Guid.NewGuid(), responseDto);
            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task Delete_ReturnsNoContent_WhenSuccessful()
        {
            var id = Guid.NewGuid();
            var responseDto = new ResponseDto
            {
                ResponseId = id,
                TextAnswer = "Answer to Delete"
            };

            _mockService.Setup(s => s.GetResponseByIdAsync(id)).ReturnsAsync(responseDto);

            var result = await _controller.Delete(id);
            Assert.That(result, Is.TypeOf<NoContentResult>());
        }

        [Test]
        public async Task Delete_ReturnsNotFound_WhenResponseDoesNotExist()
        {
            var id = Guid.NewGuid();
            _mockService.Setup(s => s.GetResponseByIdAsync(id)).ReturnsAsync((ResponseDto?)null);

            var result = await _controller.Delete(id);
            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }
    }
}
