using FSH.WebApi.Application.Common.Interfaces;
using FSH.WebApi.Application.Common.Models;
using FSH.WebApi.Application.Questions.Dtos;
using FSH.WebApi.Application.Questions;
using FSH.WebApi.Host.Controllers.Question;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Net;
using System.Security.Claims;
using Xunit;
using FSH.WebApi.Domain.Question.Enums;
using DocumentFormat.OpenXml.Presentation;
using FSH.WebApi.Domain.Question;

namespace Infrastructure.Test.Controllers.Questions;
public class QuestionControllerTest
{
    private readonly Mock<ISender> _mediatorMock;
    private readonly QuestionController _controller;
    private readonly Mock<ICurrentUser> _currentUserMock;

    public QuestionControllerTest()
    {
        _controller = new QuestionController();
        _mediatorMock = new Mock<ISender>();
        _currentUserMock = new Mock<ICurrentUser>();
        var httpContext = new DefaultHttpContext();
        httpContext.Connection.RemoteIpAddress = IPAddress.Parse("127.0.0.1");
        _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
    }

    private void SetupUserContext(string? userId, bool IsAuthenticated)
    {
        if (userId != null)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _currentUserMock.Setup(x => x.IsAuthenticated()).Returns(IsAuthenticated);
            _currentUserMock.Setup(x => x.GetUserId()).Returns(userId != null ? Guid.Parse(userId) : Guid.Empty);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal,
                    RequestServices = new ServiceCollection()
                        .AddScoped(_ => _mediatorMock.Object)
                        .BuildServiceProvider(),
                    Connection = { RemoteIpAddress = IPAddress.Parse("127.0.0.1") }
                }
            };
        }
        else
        {
            // Set up an empty context when userId is null
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }
    }

    [Theory]
    [InlineData("50d9a14d-34c8-48e4-a544-7f748f15a6e6", QuestionType.SingleChoice, "math", "CreatedAt asc")]
    [InlineData("50d9a14d-34c8-48e4-a544-7f748f15a6e6", QuestionType.MultipleChoice, "", "UpdatedOn asc")]
    [InlineData("50d9a14d-34c8-48e4-a544-7f748f15a6e6", null, "", "")]
    [InlineData("50d9a14d-34c8-48e4-a544-7f748f15a6e6", QuestionType.SingleChoice, "", "CreatedAt asc")]
    [InlineData("50d9a14d-34c8-48e4-a544-7f748f15a6e6", QuestionType.MultipleChoice, "math", "")]
    [InlineData("50d9a14d-34c8-48e4-a544-7f748f15a6e6", null, "math", "CreatedAt asc")]
    [InlineData(null, QuestionType.SingleChoice, "", "CreatedAt asc")]
    [InlineData(null, QuestionType.MultipleChoice, "", "")]
    [InlineData(null, null, "", "")]
    public async Task SearchAsync_ReturnsExpectedResult(string? folderId, QuestionType? questionType, string content, string sortBy)
    {
        // Arrange
        SetupUserContext("50d9a14d-34c8-48e4-a544-7f748f15a6e6", true);
        var request = new SearchQuestionsRequest
        {
            folderId = folderId != null ? Guid.Parse(folderId) : (Guid?)null,
            QuestionType = questionType,
            Content = content,
            PageNumber = 1,
            PageSize = 10
        };
        var mockData = new List<QuestionDto> { new QuestionDto() }; // Add mock QuestionDto objects as needed
        var expectedResponse = new PaginationResponse<QuestionDto>(mockData, 1, 1, 10);

        _mediatorMock.Setup(m => m.Send(It.IsAny<SearchQuestionsRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.SearchAsync(request);

        // Assert
        Assert.Equal(expectedResponse, result);
        _mediatorMock.Verify(m => m.Send(It.Is<SearchQuestionsRequest>(r =>
            r.folderId == request.folderId &&
            r.QuestionType == request.QuestionType &&
            r.Content == request.Content &&
            r.PageNumber == request.PageNumber &&
            r.PageSize == request.PageSize), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData("Text question?", QuestionType.SingleChoice, "Text question?", 0, "Files/Images/abc.jpg", "Files/Audio/abc.mp3", "50015e67-84cf-4a4b-b1c7-90253c05af2b", 1, "Answer Text", true, 0)]
    [InlineData("Text question?", QuestionType.MultipleChoice, "Text question?", 1, "", "", "50015e67-84cf-4a4b-b1c7-90253c05af2b", 2, "Answer Text", true, 1)]
    [InlineData("$_fillblank[2] Fill blank question.", QuestionType.FillBlank, "$_fillblank[2] Fill blank question.", 2, "", "", "50015e67-84cf-4a4b-b1c7-90253c05af2b", 1, "${1}This", true, 1)]
    [InlineData("{ \"Question\":\"abc\", \"ColumnA\":{ \"1\":\"A\", \"2\":\"B\" }, \"ColumnB\":{ \"1\":\"B\", \"2\":\"A\" } }", QuestionType.Matching, "{ \"Question\":\"abc\", \"ColumnA\":{ \"1\":\"A\", \"2\":\"B\" }, \"ColumnB\":{ \"1\":\"B\", \"2\":\"A\" } }", 3, "", "", "50015e67-84cf-4a4b-b1c7-90253c05af2b", 2, "1_2|2_1", true, 2)]
    [InlineData("Text question?", QuestionType.Reading, "Text question?", 0, "", "", "50015e67-84cf-4a4b-b1c7-90253c05af2b", 1, "Answer Text", false, 0)]
    [InlineData("Text question?", QuestionType.Reading, "Text question?", 1, "", "", "50015e67-84cf-4a4b-b1c7-90253c05af2b", 1, "Answer Text", true, 1)]
    [InlineData("Text question?", QuestionType.Writing, "Text question?", 0, "", "", "50015e67-84cf-4a4b-b1c7-90253c05af2b", 1, "Answer Text", true, 1)]
    [InlineData("Text question?", QuestionType.SingleChoice, "Text question?", 0, "", "", "50015e67-84cf-4a4b-b1c7-90253c05af2b", 2, "Answer Text", false, 0)]
    [InlineData("Text question?", QuestionType.MultipleChoice, "Text question?", 1, "", "", "50015e67-84cf-4a4b-b1c7-90253c05af2b", 1, "Answer Text", true, 1)]
    [InlineData("$_fillblank[2] Fill blank question.", QuestionType.FillBlank, "$_fillblank[2] Fill blank question.", 0, "", "", "50015e67-84cf-4a4b-b1c7-90253c05af2b", 1, "${1}This", true, 1)]
    [InlineData("{ \"Question\":\"abc\", \"ColumnA\":{ \"1\":\"A\", \"2\":\"B\" }, \"ColumnB\":{ \"1\":\"B\", \"2\":\"A\" } }", QuestionType.Matching, "{ \"Question\":\"abc\", \"ColumnA\":{ \"1\":\"A\", \"2\":\"B\" }, \"ColumnB\":{ \"1\":\"B\", \"2\":\"A\" } }", 0, "", "", "50015e67-84cf-4a4b-b1c7-90253c05af2b", 2, "1_2|2_1", true, 2)]
    [InlineData("Question Text", QuestionType.Reading, "Question Text", 1, "", "", "50015e67-84cf-4a4b-b1c7-90253c05af2b", 2, "Answer Text", false, 0)]
    [InlineData("Question Text", QuestionType.Reading, "Question Text", 2, "", "", "50015e67-84cf-4a4b-b1c7-90253c05af2b", 1, "Answer Text", true, 1)]
    [InlineData("Text question?", QuestionType.SingleChoice, "Text question?", 0, "", "", "50015e67-84cf-4a4b-b1c7-90253c05af2b", 1, "Answer Text", true, 1)]
    public async Task CreateQuestion_ReturnsExpectedResult(string content, QuestionType questionType, string questionText, int answerCount, string imagePath, string audioPath, string questionFolderId, int correctAnswerCount, string answerContent, bool? isCorrect, int? isCorrectCount)
    {
        SetupUserContext("50d9a14d-34c8-48e4-a544-7f748f15a6e6", true);
        var request = new CreateQuestionRequest
        {
            Questions = new List<CreateQuestionDto>
            {
                new CreateQuestionDto
                {
                    Content = content,
                    Image = imagePath,
                    Audio = audioPath,
                    QuestionFolderId = Guid.Parse(questionFolderId),
                    QuestionType = questionType,
                    QuestionLabelId = Guid.NewGuid(),
                    QuestionParentId = Guid.NewGuid(),
                    Answers = new List<CreateAnswerDto>
                    {
                        new CreateAnswerDto
                        {
                            Content = answerContent,
                            IsCorrect = (bool)isCorrect
                        }
                    }
                }
            }
        };

        var createdQuestionIds = new List<Guid> { Guid.NewGuid() };
        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateQuestionRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdQuestionIds);

        // Act
        var result = await _controller.CreateAsync(request);

        // Assert
        var resultList = Assert.IsType<List<Guid>>(result);
        Assert.Equal(createdQuestionIds.Count, resultList.Count);
        Assert.Equal(createdQuestionIds.First(), resultList.First());

        // Verify that the handler was called with the expected request
        _mediatorMock.Verify(m => m.Send(It.Is<CreateQuestionRequest>(r =>
            r.Questions.Count == 1 &&
            r.Questions.First().Content == content &&
            r.Questions.First().QuestionType == questionType &&
            r.Questions.First().Answers.Count == 1 &&
            r.Questions.First().Answers.First().Content == answerContent &&
            r.Questions.First().Answers.First().IsCorrect == (isCorrect ?? false)
        ), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData("dab81f37-34e6-4fd6-a049-3232f93610a1", "Text question?", QuestionType.SingleChoice, "Text question?", 0, "Files/Images/abc.jpg", "Files/Audio/abc.mp3", "50015e67-84cf-4a4b-b1c7-90253c05af2b", 1, "Answer Text", true, 0)]
    [InlineData("dab81f37-34e6-4fd6-a049-3232f93610a1", "Text question?", QuestionType.MultipleChoice, "Text question?", 1, "", "", "50015e67-84cf-4a4b-b1c7-90253c05af2b", 2, "Answer Text", true, 1)]
    [InlineData("dab81f37-34e6-4fd6-a049-3232f93610a1", "$_fillblank[2] Fill blank question.", QuestionType.FillBlank, "$_fillblank[2] Fill blank question.", 2, "", "", "50015e67-84cf-4a4b-b1c7-90253c05af2b", 1, "${1}This", true, 1)]
    [InlineData("dab81f37-34e6-4fd6-a049-3232f93610a1", "{ \"Question\":\"abc\", \"ColumnA\":{ \"1\":\"A\", \"2\":\"B\" }, \"ColumnB\":{ \"1\":\"B\", \"2\":\"A\" } }", QuestionType.Matching, "{ \"Question\":\"abc\", \"ColumnA\":{ \"1\":\"A\", \"2\":\"B\" }, \"ColumnB\":{ \"1\":\"B\", \"2\":\"A\" } }", 3, "", "", "50015e67-84cf-4a4b-b1c7-90253c05af2b", 2, "1_2|2_1", true, 2)]
    [InlineData("dab81f37-34e6-4fd6-a049-3232f93610a1", "Text question?", QuestionType.Reading, "Text question?", 0, "", "", "50015e67-84cf-4a4b-b1c7-90253c05af2b", 1, "Answer Text", false, 0)]
    [InlineData("dab81f37-34e6-4fd6-a049-3232f93610a1", "Text question?", QuestionType.Reading, "Text question?", 1, "", "", "50015e67-84cf-4a4b-b1c7-90253c05af2b", 1, "Answer Text", true, 1)]
    [InlineData("dab81f37-34e6-4fd6-a049-3232f93610a1", "Text question?", QuestionType.Writing, "Text question?", 0, "", "", "50015e67-84cf-4a4b-b1c7-90253c05af2b", 1, "Answer Text", true, 1)]
    [InlineData("dab81f37-34e6-4fd6-a049-3232f93610a1", "Text question?", QuestionType.SingleChoice, "Text question?", 0, "", "", "50015e67-84cf-4a4b-b1c7-90253c05af2b", 2, "Answer Text", false, 0)]
    [InlineData("dab81f37-34e6-4fd6-a049-3232f93610a1", "Text question?", QuestionType.MultipleChoice, "Text question?", 1, "", "", "50015e67-84cf-4a4b-b1c7-90253c05af2b", 1, "Answer Text", true, 1)]
    [InlineData("dab81f37-34e6-4fd6-a049-3232f93610a1", "$_fillblank[2] Fill blank question.", QuestionType.FillBlank, "$_fillblank[2] Fill blank question.", 0, "", "", "50015e67-84cf-4a4b-b1c7-90253c05af2b", 1, "${1}This", true, 1)]
    [InlineData("dab81f37-34e6-4fd6-a049-3232f93610a1", "{ \"Question\":\"abc\", \"ColumnA\":{ \"1\":\"A\", \"2\":\"B\" }, \"ColumnB\":{ \"1\":\"B\", \"2\":\"A\" } }", QuestionType.Matching, "{ \"Question\":\"abc\", \"ColumnA\":{ \"1\":\"A\", \"2\":\"B\" }, \"ColumnB\":{ \"1\":\"B\", \"2\":\"A\" } }", 0, "", "", "50015e67-84cf-4a4b-b1c7-90253c05af2b", 2, "1_2|2_1", true, 2)]
    [InlineData("dab81f37-34e6-4fd6-a049-3232f93610a1", "Question Text", QuestionType.Reading, "Question Text", 1, "", "", "50015e67-84cf-4a4b-b1c7-90253c05af2b", 2, "Answer Text", false, 0)]
    [InlineData("dab81f37-34e6-4fd6-a049-3232f93610a1", "Question Text", QuestionType.Reading, "Question Text", 2, "", "", "50015e67-84cf-4a4b-b1c7-90253c05af2b", 1, "Answer Text", true, 1)]
    [InlineData("dab81f37-34e6-4fd6-a049-3232f93610a1", "Text question?", QuestionType.SingleChoice, "Text question?", 0, "", "", "50015e67-84cf-4a4b-b1c7-90253c05af2b", 1, "Answer Text", true, 1)]
    public async Task UpdateQuestion_ReturnsExpectedResult(Guid questionId, string content, QuestionType questionType, string questionText, int answerCount, string imagePath, string audioPath, string questionFolderId, int correctAnswerCount, string answerContent, bool? isCorrect, int? isCorrectCount)
    {
        SetupUserContext("50d9a14d-34c8-48e4-a544-7f748f15a6e6", true);
        var request = new UpdateQuestionRequest
        {
            Id = questionId,
            Content = content,
            Image = imagePath,
            Audio = audioPath,
            QuestionFolderId = Guid.Parse(questionFolderId),
            QuestionType = questionType,
            QuestionLabelId = Guid.NewGuid(),
            ParentId = Guid.NewGuid(),
            Answers = new List<CreateAnswerDto>
            {
                new CreateAnswerDto
                {
                    Content = answerContent,
                    IsCorrect = (bool)isCorrect
                }
            }
        };

        var updatedQuestionIds = new List<Guid> { questionId };

        _mediatorMock.Setup(m => m.Send(It.Is<UpdateQuestionRequest>(r =>
        r.Id == request.Id &&
        r.Content == content &&
        r.QuestionType == questionType &&
        r.Answers.Count == 1 &&
        r.Answers.First().Content == answerContent &&
        r.Answers.First().IsCorrect == isCorrect.GetValueOrDefault(false)
    ), It.IsAny<CancellationToken>()))
    .ReturnsAsync(questionId);

        // Act
        var result = await _controller.UpdateAsync(request.Id, request);

        // Assert
        var resultAction = Assert.IsType<Guid>(questionId);
        Assert.Equal(questionId, resultAction);

        // Verify that the handler was called with the expected request
        _mediatorMock.Verify(m => m.Send(It.Is<UpdateQuestionRequest>(r =>
            r.Id == request.Id &&
            r.Content == content &&
            r.QuestionType == questionType &&
            r.Answers.Count == 1 &&
            r.Answers.First().Content == answerContent &&
            r.Answers.First().IsCorrect == isCorrect.GetValueOrDefault(false)
        ), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData("dab81f37-34e6-4fd6-a049-3232f93610a1")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("123")]
    [InlineData("412e974b-38fc-417b-8f4d-b1e577192952")]
    [InlineData("a80989f4-5f3c-4196-9932-07bb46982455")]
    public async Task DeleteQuestion_ReturnsExpectedResult(string questionId)
    {
        SetupUserContext("50d9a14d-34c8-48e4-a544-7f748f15a6e6", true);

        if (questionId == null)
        {
            // Handle null value case
            var exception = Assert.Throws<ArgumentNullException>(() => Guid.Parse(questionId));
            Assert.Equal("input", exception.ParamName);
            return;
        }

        if (string.IsNullOrEmpty(questionId) || !Guid.TryParse(questionId, out var parsedQuestionId))
        {
            // Handle invalid Guid values or skip the test
            Assert.Throws<FormatException>(() => Guid.Parse(questionId));
            return;
        }

        var request = new DeleteQuestionRequest(parsedQuestionId);

        _mediatorMock.Setup(m => m.Send(It.Is<DeleteQuestionRequest>(r =>
            r.Id == request.Id
        ), It.IsAny<CancellationToken>()))
        .ReturnsAsync(parsedQuestionId);

        // Act
        var result = await _controller.DeleteAsync(parsedQuestionId);

        // Assert
        var resultAction = Assert.IsType<Guid>(result); // Change this to match the result type of DeleteAsync
        Assert.Equal(parsedQuestionId, resultAction);

        // Verify that the handler was called with the expected request
        _mediatorMock.Verify(m => m.Send(It.Is<DeleteQuestionRequest>(r =>
            r.Id == request.Id
        ), It.IsAny<CancellationToken>()), Times.Once);
    }
}
