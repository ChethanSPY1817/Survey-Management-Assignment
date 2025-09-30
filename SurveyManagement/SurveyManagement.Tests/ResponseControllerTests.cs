using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SurveyManagement.API.Controllers;
using SurveyManagement.Application.DTOs.ResponseDTOs;
using SurveyManagement.Application.Services;
using SurveyManagement.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SurveyManagement.Tests.Controllers
{
    [TestFixture]
    public class ResponseControllerTests
    {
        private Mock<IResponseService> _serviceMock = null!;
        private ResponseController _controller = null!;

        [SetUp]
        public void Setup()
        {
            _serviceMock = new Mock<IResponseService>();
            _controller = new ResponseController(_serviceMock.Object);
        }

        [Test]
        public async Task GetAll_ReturnsOk_WithResponses()
        {
            var responses = new List<ResponseDto> { new ResponseDto { ResponseId = Guid.NewGuid() } };
            _serviceMock.Setup(s => s.GetAllResponsesAsync()).ReturnsAsync(responses);

            var result = await _controller.GetAll() as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Value, Is.EqualTo(responses));
        }

        [Test]
        public async Task GetById_ReturnsOk_WhenResponseExists()
        {
            var responseId = Guid.NewGuid();
            var response = new ResponseDto { ResponseId = responseId };
            _serviceMock.Setup(s => s.GetResponseByIdAsync(responseId)).ReturnsAsync(response);

            var result = await _controller.GetById(responseId) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Value, Is.EqualTo(response));
        }

        [Test]
        public void GetById_ThrowsNotFoundException_WhenResponseDoesNotExist()
        {
            var responseId = Guid.NewGuid();
            _serviceMock.Setup(s => s.GetResponseByIdAsync(responseId))
                        .ThrowsAsync(new NotFoundException("Response", responseId));

            Assert.That(async () => await _controller.GetById(responseId),
                        Throws.TypeOf<NotFoundException>());
        }

        [Test]
        public async Task Create_ReturnsOk_WithCreatedResponse()
        {
            var createDto = new CreateResponseDto { UserSurveyId = Guid.NewGuid() };
            var createdResponse = new ResponseDto { ResponseId = Guid.NewGuid() };
            _serviceMock.Setup(s => s.CreateResponseAsync(createDto)).ReturnsAsync(createdResponse);

            // Mock User claims
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
            }, "mock"));
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            var result = await _controller.Create(createDto) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            var value = result!.Value as ResponseDto; // cast to correct type
            Assert.That(value, Is.Not.Null);
            Assert.That(value!.ResponseId, Is.EqualTo(createdResponse.ResponseId));
        }

        [Test]
        public void Create_ThrowsBadRequestException_WhenDtoIsNull()
        {
            Assert.That(async () => await _controller.Create(null!),
                        Throws.TypeOf<BadRequestException>());
        }

        [Test]
        public async Task Update_ReturnsOk_WithUpdatedResponse()
        {
            var responseId = Guid.NewGuid();
            var responseDto = new ResponseDto { ResponseId = responseId };
            _serviceMock.Setup(s => s.UpdateResponseAsync(responseDto)).ReturnsAsync(responseDto);

            var result = await _controller.Update(responseId, responseDto) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            var value = result!.Value as ResponseDto;
            Assert.That(value, Is.Not.Null);
            Assert.That(value!.ResponseId, Is.EqualTo(responseDto.ResponseId));
        }

        [Test]
        public void Update_ThrowsBadRequestException_WhenIdMismatch()
        {
            var responseDto = new ResponseDto { ResponseId = Guid.NewGuid() };
            Assert.That(async () => await _controller.Update(Guid.NewGuid(), responseDto),
                        Throws.TypeOf<BadRequestException>());
        }

        [Test]
        public async Task Delete_ReturnsNoContent_WhenSuccessful()
        {
            var responseId = Guid.NewGuid();
            _serviceMock.Setup(s => s.DeleteResponseAsync(responseId)).Returns(Task.CompletedTask);

            var result = await _controller.Delete(responseId) as NoContentResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(204));
        }
    }
}
