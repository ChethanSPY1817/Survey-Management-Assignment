using AutoMapper;
using Moq;
using NUnit.Framework;
using SurveyManagement.Application.DTOs.ResponseDTOs;
using SurveyManagement.Application.Services;
using SurveyManagement.Domain.Entities;
using SurveyManagement.Domain.Exceptions;
using SurveyManagement.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SurveyManagement.Tests.Services
{
    [TestFixture]
    public class ResponseServiceTests
    {
        private Mock<IResponseRepository> _repoMock = null!;
        private Mock<IMapper> _mapperMock = null!;
        private ResponseService _service = null!;

        [SetUp]
        public void Setup()
        {
            _repoMock = new Mock<IResponseRepository>();
            _mapperMock = new Mock<IMapper>();
            _service = new ResponseService(_repoMock.Object, _mapperMock.Object);
        }

        [Test]
        public async Task GetAllResponsesAsync_ReturnsResponses()
        {
            var responses = new List<Response> { new Response { ResponseId = Guid.NewGuid() } };
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(responses);
            _mapperMock.Setup(m => m.Map<IEnumerable<ResponseDto>>(responses))
                       .Returns(new List<ResponseDto> { new ResponseDto { ResponseId = responses[0].ResponseId } });

            var result = await _service.GetAllResponsesAsync();

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Exactly(1).Items);
        }

        [Test]
        public async Task GetResponseByIdAsync_ReturnsResponse_WhenExists()
        {
            var responseId = Guid.NewGuid();
            var response = new Response { ResponseId = responseId };
            _repoMock.Setup(r => r.GetByIdAsync(responseId)).ReturnsAsync(response);
            _mapperMock.Setup(m => m.Map<ResponseDto>(response)).Returns(new ResponseDto { ResponseId = responseId });

            var result = await _service.GetResponseByIdAsync(responseId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.ResponseId, Is.EqualTo(responseId));
        }

        [Test]
        public void GetResponseByIdAsync_ThrowsNotFoundException_WhenNotExists()
        {
            var responseId = Guid.NewGuid();
            _repoMock.Setup(r => r.GetByIdAsync(responseId)).ReturnsAsync((Response?)null);

            Assert.That(async () => await _service.GetResponseByIdAsync(responseId),
                        Throws.TypeOf<NotFoundException>());
        }

        [Test]
        public async Task CreateResponseAsync_CreatesResponseSuccessfully()
        {
            var createDto = new CreateResponseDto { UserSurveyId = Guid.NewGuid() };

            // Map DTO to Response with dynamic GUID
            _mapperMock.Setup(m => m.Map<Response>(createDto)).Returns((CreateResponseDto d) => new Response
            {
                ResponseId = Guid.NewGuid(),
                UserSurveyId = d.UserSurveyId
            });

            _repoMock.Setup(r => r.AddAsync(It.IsAny<Response>())).Returns(Task.CompletedTask);

            // Map saved Response to ResponseDto
            _mapperMock.Setup(m => m.Map<ResponseDto>(It.IsAny<Response>())).Returns((Response r) => new ResponseDto
            {
                ResponseId = r.ResponseId,
                UserSurveyId = r.UserSurveyId
            });

            var result = await _service.CreateResponseAsync(createDto);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ResponseId, Is.Not.EqualTo(Guid.Empty));
            Assert.That(result.UserSurveyId, Is.EqualTo(createDto.UserSurveyId));
        }

        [Test]
        public async Task UpdateResponseAsync_UpdatesResponseSuccessfully()
        {
            var responseId = Guid.NewGuid();
            var responseDto = new ResponseDto { ResponseId = responseId, UserSurveyId = Guid.NewGuid() };
            var existingResponse = new Response { ResponseId = responseId };

            _repoMock.Setup(r => r.GetByIdAsync(responseId)).ReturnsAsync(existingResponse);
            _repoMock.Setup(r => r.UpdateAsync(existingResponse)).Returns(Task.CompletedTask);

            // Properly map DTO to existing entity
            _mapperMock.Setup(m => m.Map(responseDto, existingResponse)).Callback<ResponseDto, Response>((dto, entity) =>
            {
                entity.UserSurveyId = dto.UserSurveyId;
            });

            _mapperMock.Setup(m => m.Map<ResponseDto>(existingResponse)).Returns((Response r) => new ResponseDto
            {
                ResponseId = r.ResponseId,
                UserSurveyId = r.UserSurveyId
            });

            var result = await _service.UpdateResponseAsync(responseDto);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ResponseId, Is.EqualTo(responseId));
            Assert.That(result.UserSurveyId, Is.EqualTo(responseDto.UserSurveyId));
        }

        [Test]
        public void UpdateResponseAsync_ThrowsNotFoundException_WhenNotFound()
        {
            var responseDto = new ResponseDto { ResponseId = Guid.NewGuid() };
            _repoMock.Setup(r => r.GetByIdAsync(responseDto.ResponseId)).ReturnsAsync((Response?)null);

            Assert.That(async () => await _service.UpdateResponseAsync(responseDto),
                        Throws.TypeOf<NotFoundException>());
        }

        [Test]
        public async Task DeleteResponseAsync_DeletesResponseSuccessfully()
        {
            var responseId = Guid.NewGuid();
            var existingResponse = new Response { ResponseId = responseId };
            _repoMock.Setup(r => r.GetByIdAsync(responseId)).ReturnsAsync(existingResponse);
            _repoMock.Setup(r => r.DeleteAsync(responseId)).Returns(Task.CompletedTask);

            Assert.That(async () => await _service.DeleteResponseAsync(responseId), Throws.Nothing);
        }

        [Test]
        public void DeleteResponseAsync_ThrowsNotFoundException_WhenNotFound()
        {
            var responseId = Guid.NewGuid();
            _repoMock.Setup(r => r.GetByIdAsync(responseId)).ReturnsAsync((Response?)null);

            Assert.That(async () => await _service.DeleteResponseAsync(responseId),
                        Throws.TypeOf<NotFoundException>());
        }
    }
}
