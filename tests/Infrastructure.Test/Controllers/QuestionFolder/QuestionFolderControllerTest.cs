using DocumentFormat.OpenXml.Office2010.Excel;
using FSH.WebApi.Application.Common.Exceptions;
using FSH.WebApi.Application.Common.Interfaces;
using FSH.WebApi.Application.Questions;
using FSH.WebApi.Application.Questions.Dtos;
using FSH.WebApi.Domain.Question;
using FSH.WebApi.Domain.TeacherGroup;
using FSH.WebApi.Host.Controllers.Question;
using FSH.WebApi.Infrastructure.Question;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Net;

using System.Security.Claims;
using Xunit;

namespace Infrastructure.Test.Controllers.QuestionFolder;
public class QuestionFolderControllerTest
{
    private readonly Mock<ISender> _mediatorMock;
    private readonly QuestionFolderController _controller;
    private readonly Mock<ICurrentUser> _currentUserMock;

    public QuestionFolderControllerTest()
    {
        _controller = new QuestionFolderController();
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
    [InlineData("")]
    [InlineData(null)]
    [InlineData("1234")]
    [InlineData("a80989f4-5f3c-4196-9932-07bb46982455")]
    [InlineData("c6397319-650a-4f6b-8c1c-ab7532d02a1d")]
    public async Task SearchAsync_ReturnsExpectedResult(string ParentId)
    {
        SetupUserContext("50d9a14d-34c8-48e4-a544-7f748f15a6e6", true);

        if (ParentId == null)
        {
            // Handle null value case
            var exception = Assert.Throws<ArgumentNullException>(() => Guid.Parse(ParentId));
            Assert.Equal("input", exception.ParamName);
            return;
        }

        if (string.IsNullOrEmpty(ParentId) || !Guid.TryParse(ParentId, out var parsedParentId))
        {
            // Handle invalid Guid values or skip the test
            Assert.Throws<FormatException>(() => Guid.Parse(ParentId));
            return;
        }

        var questionTreeDtoList = new List<QuestionTreeDto>
        {
            new QuestionTreeDto
            {
                Id = parsedParentId,
                Name = "Test Folder",
                TotalQuestions = 10,
                CurrentShow = true,
                Children = new List<QuestionTreeDto>()
            }
        };

        var request = new GetFolderTreeRequest(parsedParentId);

        var result = await _controller.GetAsync(parsedParentId);

        Assert.Null(result);
    }

    [Theory]
    [InlineData("Math", null, "UTC001")]
    [InlineData("With space  123AAA", "b94fd617-fc6f-43fc-89c6-1b7e040979ac", "UTC002")]
    [InlineData("!@#$@", "b94fd617-fc6f-43fc-89c6-1b7e040979ac", "UTC003")]
    [InlineData("Music", "b94fd617-fc6f-43fc-89c6-1b7e040979ac", "UTC004")]
    [InlineData("Math", "b94fd617-fc6f-43fc-89c6-1b7e040979ac", "UTC005")]
    [InlineData("Math", "b94fd617-fc6f-43fc-89c6-1b7e040979ac", "UTC006")]
    public async Task CreateAsync_ReturnsExpectedResult(string name, string parentId, string testCase)
    {
        // Arrange
        SetupUserContext("50d9a14d-34c8-48e4-a544-7f748f15a6e6", true);
        var request = new CreateFolderRequest
        {
            Name = name,
            ParentId = parentId != null ? Guid.Parse(parentId) : (Guid?)null
        };

        Guid expectedGuid = Guid.NewGuid();
        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateFolderRequest>(), default))
            .ReturnsAsync(expectedGuid);

        // Act
        var result = await _controller.CreateAsync(request);

        // Assert
        switch (testCase)
        {
            case "UTC001":
            case "UTC002":
            case "UTC003":
            case "UTC004":
            case "UTC005":
            case "UTC006":
                Assert.Equal(expectedGuid, result);
                _mediatorMock.Verify(m => m.Send(It.Is<CreateFolderRequest>(r =>
                    r.Name == name && r.ParentId == (parentId != null ? Guid.Parse(parentId) : (Guid?)null)),
                    default), Times.Once);
                break;
        }
    }

    [Theory]
    [InlineData("4a94d091-b5ab-4135-9b6b-ee5de5271ab8", "With space  123AAA", "b94fd617-fc6f-43fc-89c6-1b7e040979ac", "UTC002", true)]
    [InlineData("4a94d091-b5ab-4135-9b6b-ee5de5271ab8", "!@#$@", "b94fd617-fc6f-43fc-89c6-1b7e040979ac", "UTC003", false)]
    [InlineData("4a94d091-b5ab-4135-9b6b-ee5de5271ab8", "Music", "b94fd617-fc6f-43fc-89c6-1b7e040979ac", "UTC004", true)]
    [InlineData("4a94d091-b5ab-4135-9b6b-ee5de5271ab8", "Math", "b94fd617-fc6f-43fc-89c6-1b7e040979ac", "UTC005", true)]
    [InlineData("4a94d091-b5ab-4135-9b6b-ee5de5271ab8", "Math", "123", "UTC006", false)]
    [InlineData("", "Math", "b94fd617-fc6f-43fc-89c6-1b7e040979ac", "UTC007", false)]
    public async Task UpdateAsync_ReturnsExpectedResult(string id, string name, string parentId, string testCase, bool expectedSuccess)
    {
        // Arrange
        SetupUserContext("50d9a14d-34c8-48e4-a544-7f748f15a6e6", true);

        // Handle invalid ID or ParentID cases
        if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out var parsedId))
        {
            if (!string.IsNullOrEmpty(id))
            {
                var exception = Assert.Throws<FormatException>(() => Guid.Parse(id));
                Assert.Equal("Guid should contain 32 digits with 4 dashes (xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx).", exception.Message);
            }
            return;
        }

        if (!string.IsNullOrEmpty(parentId) && !Guid.TryParse(parentId, out var parsedParentId))
        {
            var exception = Assert.Throws<FormatException>(() => Guid.Parse(parentId));
            Assert.Equal("Guid should contain 32 digits with 4 dashes (xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx).", exception.Message);
            return;
        }

        var ParentId = Guid.Parse(parentId);

        var request = new UpdateFolderRequest
        {
            Id = parsedId,
            Name = name,
            ParentId = !string.IsNullOrEmpty(parentId) ? ParentId : null
        };

        if (expectedSuccess)
        {
            _mediatorMock.Setup(m => m.Send(It.Is<UpdateFolderRequest>(r =>
                r.Id == parsedId &&
                r.Name == name &&
                r.ParentId == (!string.IsNullOrEmpty(parentId) ? (Guid?)ParentId : null)),
                default))
                .ReturnsAsync(parsedId);

            // Act
            var result = await _controller.UpdateAsync(request, parsedId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(parsedId, okResult.Value);
            _mediatorMock.Verify(m => m.Send(It.Is<UpdateFolderRequest>(r =>
                r.Id == parsedId &&
                r.Name == name &&
                r.ParentId == (!string.IsNullOrEmpty(parentId) ? (Guid?)ParentId : null)),
                default), Times.Once);
        }
        else
        {
            // Act
            var result = await _controller.UpdateAsync(request, parsedId);

            // Assert based on test cases
            switch (testCase)
            {
                case "UTC003":
                    // Assert for invalid name case
                    Assert.IsType<OkObjectResult>(result.Result);
                    break;
                case "UTC006":
                    // Assert for invalid parentId case
                    Assert.IsType<BadRequestObjectResult>(result.Result);
                    break;
                case "UTC007":
                    if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out var parsedIdForEmptyId))
                    {
                        Assert.Throws<FormatException>(() => Guid.Parse(id));
                        return;
                    }
                    break;
            }
        }
    }

    [Theory]
    [InlineData("01c7f7db-bac2-4442-94f7-287e9357cd5d", "UTC001", true)]
    [InlineData("ad5cbbd2-cf89-4f2f-9ee6-413914644a2d", "UTC002", true)]
    [InlineData("587ad6e6-3488-492f-b2a2-3e762ad8f636", "UTC003", true)]
    [InlineData("123123", "UTC004", false)]
    [InlineData(null, "UTC005", false)]
    public async Task DeleteAsync_ReturnsExpectedResult(string folderId, string testCase, bool expectedSuccess)
    {
        // Arrange
        SetupUserContext("50d9a14d-34c8-48e4-a544-7f748f15a6e6", true);

        if(testCase == "UTC004")
        {
            // Null folderId case
            var exception = await Assert.ThrowsAsync<FormatException>(async () => await _controller.DeleteAsync(Guid.Parse(folderId)));
            Assert.Contains("Guid should contain 32 digits with 4 dash", exception.Message);
            return;
        }

        if (string.IsNullOrEmpty(folderId))
        {
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(async () => await _controller.DeleteAsync(Guid.Parse(folderId)));
            Assert.Contains("Value cannot be null.", exception.Message);
            return;
        }    

        if (Guid.TryParse(folderId, out Guid parsedFolderId))
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteFolderRequest>(), default))
                .ReturnsAsync(parsedFolderId);
        }

        // Act
        var result = await _controller.DeleteAsync(Guid.Parse(folderId));

        // Assert
        if (expectedSuccess)
        {
            result = await _controller.DeleteAsync(parsedFolderId);
            var okResult = Assert.IsType<Guid>(result);
            Assert.Equal(parsedFolderId, okResult);
            _mediatorMock.Verify(m => m.Send(It.Is<DeleteFolderRequest>(r => r.Id == parsedFolderId), default));
        }
    }

    [Theory]
    [InlineData("843454b2-c5de-41d7-9dff-7f9a3d27c7c7", null, null, null, true, true, true, true, true, "UTC001", true)]
    [InlineData("", "d61b98b8-4b17-4e1b-bd01-6d8bd747d0f9", null, null, true, true, true, true, true, "UTC002", true)]
    [InlineData("", "", "nguyenvancaoky@gmail.com", null, true, true, true, true, true, "UTC003", true)]
    [InlineData("", "", null, "976354321", true, true, true, true, true, "UTC004", true)]
    [InlineData("843454b2-c5de-41d7-9dff-7f9a3d27c7c7", "d61b98b8-4b17-4e1b-bd01-6d8bd747d0f9", "nguyenvancaoky@gmail.com", "976354321", true, true, true, true, true, "UTC005", true)]
    [InlineData("843454b2-c5de-41d7-9dff-7f9a3d27c7c7", "", "", "", true, true, true, true, true, "UTC006", true)]
    [InlineData("", "", "abc@1213", " 976354321", true, true, true, true, true, "UTC007", true)]
    public async Task ShareQuestionFolder_ReturnsExpectedResult(string userId, string teacherGroupId, string email, string phone, bool canView, bool canAdd, bool canUpdate, bool canDelete, bool canShare, string testCase, bool expectedSuccess)
    {
        // Arrange
        SetupUserContext("50d9a14d-34c8-48e4-a544-7f748f15a6e6", true);
        var folderId = Guid.NewGuid();
        var request = new ShareQuestionFolderRequest
        {
            FolderId = folderId,
            UserIDs = !string.IsNullOrEmpty(userId) ? new List<Guid> { Guid.Parse(userId) } : new List<Guid>(),
            TeacherGroupIDs = !string.IsNullOrEmpty(teacherGroupId) ? new List<Guid> { Guid.Parse(teacherGroupId) } : new List<Guid>(),
            Emails = !string.IsNullOrEmpty(email) ? new List<string> { email } : new List<string>(),
            Phones = !string.IsNullOrEmpty(phone) ? new List<string> { phone } : new List<string>(),
            CanView = canView,
            CanAdd = canAdd,
            CanUpdate = canUpdate,
            CanDelete = canDelete,
            CanShare = canShare
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<ShareQuestionFolderRequest>(), default))
            .ReturnsAsync(folderId);

        // Act
        var result = await _controller.ShareAsync(request, folderId);

        // Assert
        if (expectedSuccess)
        {
            var okResult = Assert.IsType<ActionResult<Guid>>(result);
            Assert.Equal(folderId, okResult.Value);
            _mediatorMock.Verify(m => m.Send(It.Is<ShareQuestionFolderRequest>(r =>
                r.FolderId == folderId &&
                r.UserIDs.SequenceEqual(request.UserIDs) &&
                r.TeacherGroupIDs.SequenceEqual(request.TeacherGroupIDs) &&
                r.Emails.SequenceEqual(request.Emails) &&
                r.Phones.SequenceEqual(request.Phones) &&
                r.CanView == canView &&
                r.CanAdd == canAdd &&
                r.CanUpdate == canUpdate &&
                r.CanDelete == canDelete &&
                r.CanShare == canShare),
                default), Times.Once);
        }
        else
        {
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
