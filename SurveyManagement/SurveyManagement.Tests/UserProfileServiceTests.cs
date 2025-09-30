using AutoMapper;
using Moq;
using NUnit.Framework;
using SurveyManagement.Application.DTOs.UserProfileDTOs;
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
    public class UserProfileServiceTests
    {
        private Mock<IUserProfileRepository> _repositoryMock = null!;
        private Mock<IMapper> _mapperMock = null!;
        private UserProfileService _service = null!;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IUserProfileRepository>();
            _mapperMock = new Mock<IMapper>();
            _service = new UserProfileService(_repositoryMock.Object, _mapperMock.Object);
        }

        [Test]
        public async Task GetAllProfilesAsync_ReturnsProfiles()
        {
            var profiles = new List<UserProfile> { new() { UserProfileId = Guid.NewGuid(), UserId = Guid.NewGuid() } };
            _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(profiles);
            _mapperMock.Setup(m => m.Map<IEnumerable<UserProfileDto>>(profiles))
                       .Returns(new List<UserProfileDto> { new() { UserProfileId = profiles[0].UserProfileId } });

            var result = await _service.GetAllProfilesAsync();

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Exactly(1).Items);
        }

        [Test]
        public async Task GetProfileByIdAsync_ReturnsProfile_WhenExists()
        {
            var profileId = Guid.NewGuid();
            var profile = new UserProfile { UserProfileId = profileId };
            _repositoryMock.Setup(r => r.GetByIdAsync(profileId)).ReturnsAsync(profile);
            _mapperMock.Setup(m => m.Map<UserProfileDto>(profile)).Returns(new UserProfileDto { UserProfileId = profileId });

            var result = await _service.GetProfileByIdAsync(profileId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.UserProfileId, Is.EqualTo(profileId));
        }

        [Test]
        public void GetProfileByIdAsync_ThrowsNotFoundException_WhenNotFound()
        {
            var profileId = Guid.NewGuid();
            _repositoryMock.Setup(r => r.GetByIdAsync(profileId)).ReturnsAsync((UserProfile?)null);

            Assert.That(async () => await _service.GetProfileByIdAsync(profileId),
                        Throws.TypeOf<NotFoundException>());
        }

        [Test]
        public async Task CreateProfileAsync_CreatesProfileSuccessfully()
        {
            var createDto = new CreateUserProfileDto { UserId = Guid.NewGuid() };
            var profile = new UserProfile { UserProfileId = Guid.NewGuid(), UserId = createDto.UserId };

            _mapperMock.Setup(m => m.Map<UserProfile>(createDto)).Returns(profile);
            _repositoryMock.Setup(r => r.AddAsync(profile)).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<UserProfileDto>(profile)).Returns(new UserProfileDto { UserProfileId = profile.UserProfileId });

            var result = await _service.CreateProfileAsync(createDto);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.UserProfileId, Is.EqualTo(profile.UserProfileId));
        }

        [Test]
        public void CreateProfileAsync_ThrowsBadRequestException_WhenDtoIsNull()
        {
            Assert.That(async () => await _service.CreateProfileAsync(null!),
                        Throws.TypeOf<BadRequestException>());
        }

        [Test]
        public async Task UpdateProfileAsync_UpdatesProfileSuccessfully()
        {
            var profileId = Guid.NewGuid();
            var profileDto = new UserProfileDto { UserProfileId = profileId, FirstName = "NewName" };
            var profile = new UserProfile { UserProfileId = profileId, FirstName = "OldName" };

            _repositoryMock.Setup(r => r.GetByIdAsync(profileId)).ReturnsAsync(profile);
            _repositoryMock.Setup(r => r.UpdateAsync(profile)).Returns(Task.CompletedTask);

            Assert.That(async () => await _service.UpdateProfileAsync(profileDto), Throws.Nothing);
            Assert.That(profile.FirstName, Is.EqualTo("NewName"));
        }

        [Test]
        public void UpdateProfileAsync_ThrowsNotFoundException_WhenProfileNotFound()
        {
            var profileDto = new UserProfileDto { UserProfileId = Guid.NewGuid() };
            _repositoryMock.Setup(r => r.GetByIdAsync(profileDto.UserProfileId)).ReturnsAsync((UserProfile?)null);

            Assert.That(async () => await _service.UpdateProfileAsync(profileDto),
                        Throws.TypeOf<NotFoundException>());
        }

        [Test]
        public async Task DeleteProfileAsync_DeletesProfileSuccessfully()
        {
            var profileId = Guid.NewGuid();
            _repositoryMock.Setup(r => r.GetByIdAsync(profileId)).ReturnsAsync(new UserProfile { UserProfileId = profileId });
            _repositoryMock.Setup(r => r.DeleteAsync(profileId)).Returns(Task.CompletedTask);

            Assert.That(async () => await _service.DeleteProfileAsync(profileId), Throws.Nothing);
        }

        [Test]
        public void DeleteProfileAsync_ThrowsNotFoundException_WhenProfileNotFound()
        {
            var profileId = Guid.NewGuid();
            _repositoryMock.Setup(r => r.GetByIdAsync(profileId)).ReturnsAsync((UserProfile?)null);

            Assert.That(async () => await _service.DeleteProfileAsync(profileId),
                        Throws.TypeOf<NotFoundException>());
        }
    }
}
