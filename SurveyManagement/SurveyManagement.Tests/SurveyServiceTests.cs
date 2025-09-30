using AutoMapper;
using Moq;
using NUnit.Framework;
using SurveyManagement.Application.DTOs.SurveyDTOs;
using SurveyManagement.Application.Services;
using SurveyManagement.Domain.Entities;
using SurveyManagement.Domain.Exceptions;
using SurveyManagement.Domain.Interfaces;
using SurveyManagement.CrossCutting.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SurveyManagement.Tests.Services
{
    [TestFixture]
    public class SurveyServiceTests
    {
        private Mock<ISurveyRepository> _repositoryMock = null!;
        private Mock<IMapper> _mapperMock = null!;
        private Mock<IServiceLogger<SurveyService>> _loggerMock = null!;
        private SurveyService _service = null!;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<ISurveyRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<IServiceLogger<SurveyService>>();
            _service = new SurveyService(_repositoryMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task GetAllAsync_ReturnsSurveys()
        {
            var surveys = new List<Survey> { new Survey { SurveyId = Guid.NewGuid() } };
            _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(surveys);
            _mapperMock.Setup(m => m.Map<SurveyDto>(It.IsAny<Survey>()))
                       .Returns((Survey s) => new SurveyDto { SurveyId = s.SurveyId });

            var result = await _service.GetAllAsync();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task GetByIdAsync_ReturnsSurvey_WhenExists()
        {
            var surveyId = Guid.NewGuid();
            var survey = new Survey { SurveyId = surveyId };
            _repositoryMock.Setup(r => r.GetByIdAsync(surveyId)).ReturnsAsync(survey);
            _mapperMock.Setup(m => m.Map<SurveyDto>(survey)).Returns(new SurveyDto { SurveyId = surveyId });

            var result = await _service.GetByIdAsync(surveyId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.SurveyId, Is.EqualTo(surveyId));
        }

        [Test]
        public void GetByIdAsync_ThrowsNotFoundException_WhenSurveyNotFound()
        {
            var surveyId = Guid.NewGuid();
            _repositoryMock.Setup(r => r.GetByIdAsync(surveyId)).ReturnsAsync((Survey?)null);

            Assert.That(async () => await _service.GetByIdAsync(surveyId),
                        Throws.TypeOf<NotFoundException>());
        }

        [Test]
        public async Task CreateAsync_CreatesSurveySuccessfully()
        {
            var createDto = new CreateSurveyDto { Title = "Survey" };
            var currentUserId = Guid.NewGuid();
            var survey = new Survey { SurveyId = Guid.NewGuid(), Title = createDto.Title, CreatedByUserId = currentUserId };
            _mapperMock.Setup(m => m.Map<Survey>(createDto)).Returns(survey);
            _repositoryMock.Setup(r => r.AddAsync(survey)).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<SurveyDto>(survey)).Returns(new SurveyDto { SurveyId = survey.SurveyId });

            var result = await _service.CreateAsync(createDto, currentUserId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.SurveyId, Is.EqualTo(survey.SurveyId));
        }

        [Test]
        public async Task UpdateAsync_UpdatesSurveySuccessfully()
        {
            var currentUserId = Guid.NewGuid();
            var surveyId = Guid.NewGuid();
            var existingSurvey = new Survey { SurveyId = surveyId, CreatedByUserId = currentUserId };
            var surveyDto = new SurveyDto { SurveyId = surveyId, Title = "Updated" };

            _repositoryMock.Setup(r => r.GetByIdAsync(surveyId)).ReturnsAsync(existingSurvey);
            _repositoryMock.Setup(r => r.UpdateAsync(existingSurvey)).Returns(Task.CompletedTask);

            Assert.That(async () => await _service.UpdateAsync(surveyDto, currentUserId), Throws.Nothing);
            Assert.That(existingSurvey.Title, Is.EqualTo("Updated"));
        }

        [Test]
        public void UpdateAsync_ThrowsUnauthorizedException_WhenUserNotOwner()
        {
            var surveyId = Guid.NewGuid();
            var surveyDto = new SurveyDto { SurveyId = surveyId };
            var existingSurvey = new Survey { SurveyId = surveyId, CreatedByUserId = Guid.NewGuid() };
            _repositoryMock.Setup(r => r.GetByIdAsync(surveyId)).ReturnsAsync(existingSurvey);

            Assert.That(async () => await _service.UpdateAsync(surveyDto, Guid.NewGuid()),
                        Throws.TypeOf<UnauthorizedException>());
        }

        [Test]
        public void UpdateAsync_ThrowsNotFoundException_WhenSurveyNotFound()
        {
            var surveyId = Guid.NewGuid();
            var surveyDto = new SurveyDto { SurveyId = surveyId };
            _repositoryMock.Setup(r => r.GetByIdAsync(surveyId)).ReturnsAsync((Survey?)null);

            Assert.That(async () => await _service.UpdateAsync(surveyDto, Guid.NewGuid()),
                        Throws.TypeOf<NotFoundException>());
        }

        [Test]
        public async Task DeleteAsync_DeletesSurveySuccessfully()
        {
            var surveyId = Guid.NewGuid();
            var currentUserId = Guid.NewGuid();
            var survey = new Survey { SurveyId = surveyId, CreatedByUserId = currentUserId };

            _repositoryMock.Setup(r => r.GetByIdAsync(surveyId)).ReturnsAsync(survey);
            _repositoryMock.Setup(r => r.DeleteAsync(surveyId)).Returns(Task.CompletedTask);

            Assert.That(async () => await _service.DeleteAsync(surveyId, currentUserId), Throws.Nothing);
        }

        [Test]
        public void DeleteAsync_ThrowsUnauthorizedException_WhenUserNotOwner()
        {
            var surveyId = Guid.NewGuid();
            var survey = new Survey { SurveyId = surveyId, CreatedByUserId = Guid.NewGuid() };
            _repositoryMock.Setup(r => r.GetByIdAsync(surveyId)).ReturnsAsync(survey);

            Assert.That(async () => await _service.DeleteAsync(surveyId, Guid.NewGuid()),
                        Throws.TypeOf<UnauthorizedException>());
        }
    }
}
