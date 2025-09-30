using AutoMapper;
using Moq;
using NUnit.Framework;
using SurveyManagement.Application.DTOs.OptionDTOs;
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
    public class OptionServiceTests
    {
        private Mock<IOptionRepository> _repoMock = null!;
        private Mock<IMapper> _mapperMock = null!;
        private OptionService _service = null!;

        [SetUp]
        public void Setup()
        {
            _repoMock = new Mock<IOptionRepository>();
            _mapperMock = new Mock<IMapper>();
            _service = new OptionService(_repoMock.Object, _mapperMock.Object);
        }

        [Test]
        public async Task GetAllByQuestionIdAsync_ReturnsOptions()
        {
            var questionId = Guid.NewGuid();
            var options = new List<Option> { new Option { OptionId = Guid.NewGuid(), Text = "Option1" } };
            _repoMock.Setup(r => r.GetAllByQuestionIdAsync(questionId)).ReturnsAsync(options);
            _mapperMock.Setup(m => m.Map<IEnumerable<OptionDto>>(options)).Returns(new List<OptionDto> { new OptionDto { OptionId = options[0].OptionId } });

            var result = await _service.GetAllByQuestionIdAsync(questionId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Exactly(1).Items);
        }

        [Test]
        public async Task GetByIdAsync_ReturnsOption_WhenExists()
        {
            var optionId = Guid.NewGuid();
            var option = new Option { OptionId = optionId, Text = "Test" };
            _repoMock.Setup(r => r.GetByIdAsync(optionId)).ReturnsAsync(option);
            _mapperMock.Setup(m => m.Map<OptionDto>(option)).Returns(new OptionDto { OptionId = optionId });

            var result = await _service.GetByIdAsync(optionId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.OptionId, Is.EqualTo(optionId));
        }

        [Test]
        public void GetByIdAsync_ThrowsNotFoundException_WhenNotExists()
        {
            var optionId = Guid.NewGuid();
            _repoMock.Setup(r => r.GetByIdAsync(optionId)).ReturnsAsync((Option?)null);

            Assert.That(async () => await _service.GetByIdAsync(optionId),
                        Throws.TypeOf<NotFoundException>());
        }

        [Test]
        public async Task CreateOptionAsync_CreatesOptionSuccessfully()
        {
            var createDto = new CreateOptionDto { Text = "New", Order = 1 };

            // Setup mapping to create Option entity with a new GUID
            _mapperMock.Setup(m => m.Map<Option>(createDto)).Returns((CreateOptionDto dto) => new Option
            {
                OptionId = Guid.NewGuid(),
                Text = dto.Text,
                Order = dto.Order
            });

            _repoMock.Setup(r => r.AddAsync(It.IsAny<Option>())).Returns(Task.CompletedTask);

            // Map the saved entity back to DTO
            _mapperMock.Setup(m => m.Map<OptionDto>(It.IsAny<Option>())).Returns((Option entity) => new OptionDto
            {
                OptionId = entity.OptionId,
                Text = entity.Text,
                Order = entity.Order
            });

            var result = await _service.CreateOptionAsync(createDto);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.OptionId, Is.Not.EqualTo(Guid.Empty));
            Assert.That(result.Text, Is.EqualTo(createDto.Text));
            Assert.That(result.Order, Is.EqualTo(createDto.Order));
        }

        [Test]
        public async Task UpdateOptionAsync_UpdatesOptionSuccessfully()
        {
            var optionId = Guid.NewGuid();
            var updateDto = new UpdateOptionDto { OptionId = optionId, Text = "Updated", Order = 2 };
            var existingOption = new Option { OptionId = optionId, Text = "Old", Order = 1 };

            _repoMock.Setup(r => r.GetByIdAsync(optionId)).ReturnsAsync(existingOption);
            _repoMock.Setup(r => r.UpdateAsync(existingOption)).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<OptionDto>(existingOption)).Returns(new OptionDto { OptionId = optionId, Text = "Updated", Order = 2 });

            var result = await _service.UpdateOptionAsync(updateDto);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Text, Is.EqualTo("Updated"));
            Assert.That(result.Order, Is.EqualTo(2));
        }

        [Test]
        public void UpdateOptionAsync_ThrowsNotFoundException_WhenNotFound()
        {
            var updateDto = new UpdateOptionDto { OptionId = Guid.NewGuid(), Text = "X" };
            _repoMock.Setup(r => r.GetByIdAsync(updateDto.OptionId)).ReturnsAsync((Option?)null);

            Assert.That(async () => await _service.UpdateOptionAsync(updateDto),
                        Throws.TypeOf<NotFoundException>());
        }

        [Test]
        public async Task DeleteOptionAsync_DeletesOptionSuccessfully()
        {
            var optionId = Guid.NewGuid();
            var existingOption = new Option { OptionId = optionId };
            _repoMock.Setup(r => r.GetByIdAsync(optionId)).ReturnsAsync(existingOption);
            _repoMock.Setup(r => r.DeleteAsync(optionId)).Returns(Task.CompletedTask);

            Assert.That(async () => await _service.DeleteOptionAsync(optionId), Throws.Nothing);
        }

        [Test]
        public void DeleteOptionAsync_ThrowsNotFoundException_WhenNotFound()
        {
            var optionId = Guid.NewGuid();
            _repoMock.Setup(r => r.GetByIdAsync(optionId)).ReturnsAsync((Option?)null);

            Assert.That(async () => await _service.DeleteOptionAsync(optionId),
                        Throws.TypeOf<NotFoundException>());
        }
    }
}
