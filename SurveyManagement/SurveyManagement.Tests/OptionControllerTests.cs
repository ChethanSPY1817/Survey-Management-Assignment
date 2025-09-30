using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SurveyManagement.API.Controllers;
using SurveyManagement.Application.DTOs.OptionDTOs;
using SurveyManagement.Application.Services;
using SurveyManagement.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SurveyManagement.Tests.Controllers
{
    [TestFixture]
    public class OptionControllerTests
    {
        private Mock<IOptionService> _serviceMock = null!;
        private OptionController _controller = null!;

        [SetUp]
        public void Setup()
        {
            _serviceMock = new Mock<IOptionService>();
            _controller = new OptionController(_serviceMock.Object);
        }

        [Test]
        public async Task GetAllByQuestionId_ReturnsOk_WithOptions()
        {
            var questionId = Guid.NewGuid();
            var options = new List<OptionDto> { new OptionDto { OptionId = Guid.NewGuid(), Text = "Option1" } };
            _serviceMock.Setup(s => s.GetAllByQuestionIdAsync(questionId)).ReturnsAsync(options);

            var result = await _controller.GetAllByQuestionId(questionId) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Value, Is.EqualTo(options));
        }

        [Test]
        public async Task GetById_ReturnsOk_WhenOptionExists()
        {
            var optionId = Guid.NewGuid();
            var option = new OptionDto { OptionId = optionId, Text = "OptionTest" };
            _serviceMock.Setup(s => s.GetByIdAsync(optionId)).ReturnsAsync(option);

            var result = await _controller.GetById(optionId) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Value, Is.EqualTo(option));
        }

        [Test]
        public void GetById_ThrowsNotFoundException_WhenOptionDoesNotExist()
        {
            var optionId = Guid.NewGuid();
            _serviceMock.Setup(s => s.GetByIdAsync(optionId)).ThrowsAsync(new NotFoundException("Option", optionId));

            Assert.That(async () => await _controller.GetById(optionId),
                        Throws.TypeOf<NotFoundException>());
        }

        [Test]
        public async Task Create_ReturnsOk_WithCreatedOption()
        {
            var createDto = new CreateOptionDto { Text = "New Option", Order = 1 };
            var createdOption = new OptionDto { OptionId = Guid.NewGuid(), Text = createDto.Text };
            _serviceMock.Setup(s => s.CreateOptionAsync(createDto)).ReturnsAsync(createdOption);

            var result = await _controller.Create(createDto) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Value, Is.EqualTo(createdOption));
        }

        [Test]
        public void Create_ThrowsBadRequestException_WhenDtoIsNull()
        {
            Assert.That(async () => await _controller.Create(null!),
                        Throws.TypeOf<BadRequestException>());
        }

        [Test]
        public async Task Update_ReturnsOk_WithUpdatedOption()
        {
            var optionId = Guid.NewGuid();
            var updateDto = new UpdateOptionDto { OptionId = optionId, Text = "Updated", Order = 2 };
            var updatedOption = new OptionDto { OptionId = optionId, Text = "Updated" };
            _serviceMock.Setup(s => s.UpdateOptionAsync(updateDto)).ReturnsAsync(updatedOption);

            var result = await _controller.Update(optionId, updateDto) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Value, Is.EqualTo(updatedOption));
        }

        [Test]
        public void Update_ThrowsBadRequestException_WhenIdMismatch()
        {
            var updateDto = new UpdateOptionDto { OptionId = Guid.NewGuid(), Text = "Mismatch" };
            Assert.That(async () => await _controller.Update(Guid.NewGuid(), updateDto),
                        Throws.TypeOf<BadRequestException>());
        }

        [Test]
        public async Task Delete_ReturnsNoContent_WhenSuccessful()
        {
            var optionId = Guid.NewGuid();
            _serviceMock.Setup(s => s.DeleteOptionAsync(optionId)).Returns(Task.CompletedTask);

            var result = await _controller.Delete(optionId) as NoContentResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(204));
        }
    }
}
