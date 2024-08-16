using System.Net;
using FSH.WebApi.Application.Common.Exceptions;
using FSH.WebApi.Application.Identity.Tokens;
using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Host.Controllers.Identity;
using FSH.WebApi.Infrastructure.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Moq;
using Xunit;

namespace Infrastructure.Test.Controllers.Identity;
public class UsersControllerTest
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly UsersController _controller;
    private readonly IStringLocalizer _t;

    public UsersControllerTest()
    {
        _userServiceMock = new Mock<IUserService>();
        _controller = new UsersController(_userServiceMock.Object);
        _t = new Mock<IStringLocalizer>().Object;
    }

    [Fact]
    public async Task GetByIdAsync_ValidId_ReturnsUserDetails()
    {
        // Arrange
        var userId = "5b15f43e-41a9-4ce7-a86f-dd646c22079e";
        var userDetails = new UserDetailsDto { Id = Guid.Parse(userId) };
        _userServiceMock.Setup(x => x.GetAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userDetails);

        // Act
        var result = await _controller.GetByIdAsync(userId, CancellationToken.None);

        // Assert
        Assert.Equal(userDetails, result);
    }

    [Fact]
    public async Task GetByIdAsync_InvalidId_ThrowsNotFoundException()
    {
        // Arrange
        var invalidId = "123";
        _userServiceMock.Setup(x => x.GetAsync(invalidId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException(_t["User Not Found"]));

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _controller.GetByIdAsync(invalidId, CancellationToken.None));
    }

    [Fact]
    public async Task GetByIdAsync_NullId_ReturnsFalse()
    {
        // Arrange
        string nullId = null;
        _userServiceMock.Setup(x => x.GetAsync(nullId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserDetailsDto)null);

        // Act
        var result = await _controller.GetByIdAsync(nullId, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistentId_ThrowsNotFoundException()
    {
        // Arrange
        var nonExistentId = "d311-fcd5-4878-be4b-a4490574b08d";
        _userServiceMock.Setup(x => x.GetAsync(nonExistentId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException(_t["User Not Found"]));

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _controller.GetByIdAsync(nonExistentId, CancellationToken.None));
    }

    [Fact]
    public async Task GetByIdAsync_ValidIdUTC5_ReturnsUserDetails()
    {
        // Arrange
        var userId = "21683b36-3526-4260-bea8-6c6a8e6b63d5";
        var userDetails = new UserDetailsDto { Id = Guid.Parse(userId) };
        _userServiceMock.Setup(x => x.GetAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userDetails);

        // Act
        var result = await _controller.GetByIdAsync(userId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<UserDetailsDto>(result);
        Assert.Equal(userId, result.Id.ToString());
    }
}
