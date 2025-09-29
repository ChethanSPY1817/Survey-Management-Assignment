using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SurveyManagement.API.Controllers;
using SurveyManagement.Application.DTOs.QuestionDTOs;
using SurveyManagement.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SurveyManagement.Tests
{
    [TestFixture]
    public class QuestionControllerTests
    {
        private Mock<IQuestionService> _mockService = null!;
        private QuestionController _controller = null!;

        [SetUp]
        public void Setup()
        {
            _mockService = new Mock<IQuestionService>();
            _controller = new QuestionController(_mockService.Object);
        }

        [Test]
        public async Task GetAllBySurveyId_ReturnsOkResult_WithQuestions()
        {
            var surveyId = Guid.NewGuid();
            _mockService.Setup(s => s.GetAllBySurveyIdAsync(surveyId))
                .ReturnsAsync(new List<QuestionDto>
                {
                    new QuestionDto { QuestionId = Guid.NewGuid(), Text = "Question 1" }
                });

            var result = await _controller.GetAllBySurveyId(surveyId);
            var okResult = result as OkObjectResult;
            var questions = okResult?.Value as IEnumerable<QuestionDto>;

            Assert.That(okResult, Is.Not.Null);
            Assert.That(questions?.Count() ?? 0, Is.EqualTo(1));
        }

        [Test]
        public async Task GetById_ReturnsOkResult_WhenQuestionExists()
        {
            var id = Guid.NewGuid();
            _mockService.Setup(s => s.GetByIdAsync(id))
                .ReturnsAsync(new QuestionDto { QuestionId = id, Text = "Sample Question" });

            var result = await _controller.GetById(id);
            var okResult = result as OkObjectResult;
            var question = okResult?.Value as QuestionDto;

            Assert.That(okResult, Is.Not.Null);
            Assert.That(question?.QuestionId, Is.EqualTo(id));
        }

        [Test]
        public async Task GetById_ReturnsNotFound_WhenQuestionDoesNotExist()
        {
            var id = Guid.NewGuid();
            _mockService.Setup(s => s.GetByIdAsync(id))
                .ReturnsAsync((QuestionDto?)null);

            var result = await _controller.GetById(id);
            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public async Task Create_ReturnsCreatedAtAction()
        {
            var createDto = new CreateQuestionDto { Text = "New Question", SurveyId = Guid.NewGuid() };
            var createdQuestion = new QuestionDto { QuestionId = Guid.NewGuid(), Text = "New Question" };

            _mockService.Setup(s => s.CreateAsync(createDto))
                .ReturnsAsync(createdQuestion);

            var result = await _controller.Create(createDto);
            var createdResult = result as CreatedAtActionResult;

            Assert.That(createdResult, Is.Not.Null);
            Assert.That(createdResult?.Value, Is.EqualTo(createdQuestion));
        }

        [Test]
        public async Task Update_ReturnsNoContent_WhenSuccessful()
        {
            var id = Guid.NewGuid();
            var updateDto = new UpdateQuestionDto { QuestionId = id, Text = "Updated Question" };

            _mockService.Setup(s => s.GetByIdAsync(id))
                .ReturnsAsync(new QuestionDto { QuestionId = id, Text = "Old Question" });

            var result = await _controller.Update(id, updateDto);
            Assert.That(result, Is.TypeOf<NoContentResult>());
        }

        [Test]
        public async Task Update_ReturnsBadRequest_WhenIdMismatch()
        {
            var updateDto = new UpdateQuestionDto { QuestionId = Guid.NewGuid(), Text = "Updated Question" };
            var result = await _controller.Update(Guid.NewGuid(), updateDto);

            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task Delete_ReturnsNoContent_WhenSuccessful()
        {
            var id = Guid.NewGuid();
            _mockService.Setup(s => s.GetByIdAsync(id))
                .ReturnsAsync(new QuestionDto { QuestionId = id, Text = "Question to Delete" });

            var result = await _controller.Delete(id);
            Assert.That(result, Is.TypeOf<NoContentResult>());
        }

        [Test]
        public async Task Delete_ReturnsNotFound_WhenQuestionDoesNotExist()
        {
            var id = Guid.NewGuid();
            _mockService.Setup(s => s.GetByIdAsync(id))
                .ReturnsAsync((QuestionDto?)null);

            var result = await _controller.Delete(id);
            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }
    }
}
