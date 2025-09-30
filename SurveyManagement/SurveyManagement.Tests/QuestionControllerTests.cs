using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SurveyManagement.API.Controllers;
using SurveyManagement.Application.DTOs.QuestionDTOs;
using SurveyManagement.Application.Services;
using SurveyManagement.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SurveyManagement.Tests.Controllers
{
    [TestFixture]
    public class QuestionControllerTests
    {
        private Mock<IQuestionService> _serviceMock = null!;
        private QuestionController _controller = null!;

        [SetUp]
        public void Setup()
        {
            _serviceMock = new Mock<IQuestionService>();
            _controller = new QuestionController(_serviceMock.Object);
        }

        [Test]
        public async Task GetAllBySurveyId_ReturnsOk_WithQuestions()
        {
            var surveyId = Guid.NewGuid();
            var questions = new List<QuestionDto> { new QuestionDto { QuestionId = Guid.NewGuid() } };
            _serviceMock.Setup(s => s.GetAllBySurveyIdAsync(surveyId)).ReturnsAsync(questions);

            var result = await _controller.GetAllBySurveyId(surveyId) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Value, Is.EqualTo(questions));
        }

        [Test]
        public async Task GetById_ReturnsOk_WhenQuestionExists()
        {
            var questionId = Guid.NewGuid();
            var question = new QuestionDto { QuestionId = questionId };
            _serviceMock.Setup(s => s.GetByIdAsync(questionId)).ReturnsAsync(question);

            var result = await _controller.GetById(questionId) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Value, Is.EqualTo(question));
        }

        [Test]
        public void GetById_ThrowsNotFoundException_WhenQuestionNotFound()
        {
            var questionId = Guid.NewGuid();
            _serviceMock.Setup(s => s.GetByIdAsync(questionId)).ReturnsAsync((QuestionDto?)null);

            Assert.That(async () => await _controller.GetById(questionId),
                        Throws.TypeOf<NotFoundException>());
        }

        [Test]
        public async Task Create_ReturnsOk_WithCreatedQuestion()
        {
            var createDto = new CreateQuestionDto { Text = "Q1" };
            var question = new QuestionDto { QuestionId = Guid.NewGuid() };
            _serviceMock.Setup(s => s.CreateAsync(createDto)).ReturnsAsync(question);

            var result = await _controller.Create(createDto) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Value, Is.EqualTo(question));
        }

        [Test]
        public void Create_ThrowsBadRequestException_WhenDtoIsNull()
        {
            Assert.That(async () => await _controller.Create(null!),
                        Throws.TypeOf<BadRequestException>());
        }

        [Test]
        public async Task Update_ReturnsNoContent_WhenSuccessful()
        {
            var questionId = Guid.NewGuid();
            var updateDto = new UpdateQuestionDto { QuestionId = questionId };
            _serviceMock.Setup(s => s.UpdateAsync(updateDto)).Returns(Task.CompletedTask);

            var result = await _controller.Update(questionId, updateDto) as NoContentResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(204));
        }

        [Test]
        public void Update_ThrowsBadRequestException_WhenIdMismatch()
        {
            var updateDto = new UpdateQuestionDto { QuestionId = Guid.NewGuid() };
            Assert.That(async () => await _controller.Update(Guid.NewGuid(), updateDto),
                        Throws.TypeOf<BadRequestException>());
        }

        [Test]
        public async Task Delete_ReturnsNoContent_WhenSuccessful()
        {
            var questionId = Guid.NewGuid();
            _serviceMock.Setup(s => s.DeleteAsync(questionId)).Returns(Task.CompletedTask);

            var result = await _controller.Delete(questionId) as NoContentResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(204));
        }
    }
}
