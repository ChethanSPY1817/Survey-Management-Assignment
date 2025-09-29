using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SurveyManagement.API.Controllers;
using SurveyManagement.Application.DTOs.OptionDTOs;
using SurveyManagement.Application.Services;

[TestFixture]
public class OptionControllerTests
{
    private Mock<IOptionService> _mockService = null!;
    private OptionController _controller = null!;

    [SetUp]
    public void Setup()
    {
        _mockService = new Mock<IOptionService>();
        _controller = new OptionController(_mockService.Object);
    }

    [Test]
    public async Task GetAllByQuestionId_ReturnsOk_WithOptions()
    {
        var questionId = Guid.NewGuid();
        _mockService.Setup(s => s.GetAllByQuestionIdAsync(questionId))
            .ReturnsAsync(new List<OptionDto> { new OptionDto { Text = "Option 1" } });

        var result = await _controller.GetAllByQuestionId(questionId);
        var okResult = result as OkObjectResult;
        var options = okResult?.Value as IEnumerable<OptionDto>;

        Assert.That(okResult, Is.Not.Null);
        Assert.That(options?.Count() ?? 0, Is.EqualTo(1));
    }

    [Test]
    public async Task GetById_ReturnsNotFound_WhenOptionDoesNotExist()
    {
        var id = Guid.NewGuid();
        _mockService.Setup(s => s.GetByIdAsync(id)).ReturnsAsync((OptionDto?)null);

        var result = await _controller.GetById(id);
        Assert.That(result, Is.TypeOf<NotFoundResult>());
    }

    [Test]
    public async Task Create_ReturnsOk_WhenOptionCreated()
    {
        var createDto = new CreateOptionDto { QuestionId = Guid.NewGuid(), Text = "New Option" };

        var result = await _controller.Create(createDto);
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
    }
}
