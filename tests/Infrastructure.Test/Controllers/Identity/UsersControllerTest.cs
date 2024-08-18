using System.Net;
using System.Security.Claims;
using DocumentFormat.OpenXml.Spreadsheet;
using FSH.WebApi.Application.Common.Exceptions;
using FSH.WebApi.Application.Common.Interfaces;
using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Application.Identity.Users.Verify;
using FSH.WebApi.Host.Controllers.Identity;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Moq;
using Xunit;

namespace Infrastructure.Test.Controllers.Identity;
public class UsersControllerTest
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<ISender> _mediatorMock;
    private readonly Mock<ICurrentUser> _currentUserMock;
    private readonly UsersController _controller;
    private readonly IStringLocalizer _t;

    public UsersControllerTest()
    {
        _userServiceMock = new Mock<IUserService>();
        _mediatorMock = new Mock<ISender>();
        _currentUserMock = new Mock<ICurrentUser>();
        _controller = new UsersController(_userServiceMock.Object);
        _t = new Mock<IStringLocalizer>().Object;
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
                        .BuildServiceProvider()
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

    // Test for the GetUserDetail

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

    // Test for the ConfrimEmailAddressForUser

    [Fact]
    public async Task ConfirmEmailAsync_ValidUserIdAndCode_ReturnsTrue()
    {
        // Arrange
        var userId = "5b15f43e-41a9-4ce7-a86f-dd646c22079e";
        var code = "32bc02d0-3641-4c82-9e55-977ac12caab8";
        var tenant = "root";
        _userServiceMock.Setup(x => x.ConfirmEmailAsync(userId, code, tenant, It.IsAny<CancellationToken>()))
            .ReturnsAsync("Account Confirmed for E-Mail test@example.com. You can now use the /api/tokens endpoint to generate JWT.");

        // Act
        var result = await _controller.ConfirmEmailAsync(tenant, userId, code, CancellationToken.None);

        // Assert
        Assert.Contains("Account Confirmed", result);
    }

    [Fact]
    public async Task ConfirmEmailAsync_InvalidUserId_ReturnsFalse()
    {
        // Arrange
        var userId = "123";
        var code = "32bc02d0-3641-4c82-9e55-977ac12caab8";
        var tenant = "root";
        _userServiceMock.Setup(x => x.ConfirmEmailAsync(userId, code, tenant, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InternalServerException("An error occurred while confirming E-Mail."));

        // Act & Assert
        await Assert.ThrowsAsync<InternalServerException>(() =>
            _controller.ConfirmEmailAsync(tenant, userId, code, CancellationToken.None));
    }

    [Fact]
    public async Task ConfirmEmailAsync_ValidUserIdInvalidCode_ReturnsFalse()
    {
        // Arrange
        var userId = "5b15f43e-41a9-4ce7-a86f-dd646c22079e";
        var code = "123";
        var tenant = "root";
        _userServiceMock.Setup(x => x.ConfirmEmailAsync(userId, code, tenant, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InternalServerException("An error occurred while confirming E-Mail."));

        // Act & Assert
        await Assert.ThrowsAsync<InternalServerException>(() =>
            _controller.ConfirmEmailAsync(tenant, userId, code, CancellationToken.None));
    }

    [Fact]
    public async Task ConfirmEmailAsync_InvalidUserIdAndCode_ReturnsFalse()
    {
        // Arrange
        var userId = "123";
        var code = "123";
        var tenant = "root";
        _userServiceMock.Setup(x => x.ConfirmEmailAsync(userId, code, tenant, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InternalServerException("An error occurred while confirming E-Mail."));

        // Act & Assert
        await Assert.ThrowsAsync<InternalServerException>(() =>
            _controller.ConfirmEmailAsync(tenant, userId, code, CancellationToken.None));
    }

    [Fact]
    public async Task ConfirmEmailAsync_NullUserId_ReturnsFalse()
    {
        // Arrange
        string userId = null;
        var code = "32bc02d0-3641-4c82-9e55-977ac12caab8";
        var tenant = "root";
        _userServiceMock.Setup(x => x.ConfirmEmailAsync(userId, code, tenant, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InternalServerException("An error occurred while confirming E-Mail."));

        // Act & Assert
        await Assert.ThrowsAsync<InternalServerException>(() =>
            _controller.ConfirmEmailAsync(tenant, userId, code, CancellationToken.None));
    }

    [Fact]
    public async Task ConfirmEmailAsync_NullUserIdAndValidCode_ReturnsFalse()
    {
        // Arrange
        string userId = null;
        var code = "32bc02d0-3641-4c82-9e55-977ac12caab8";
        var tenant = "root";
        _userServiceMock.Setup(x => x.ConfirmEmailAsync(userId, code, tenant, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InternalServerException("An error occurred while confirming E-Mail."));

        // Act & Assert
        await Assert.ThrowsAsync<InternalServerException>(() =>
            _controller.ConfirmEmailAsync(tenant, userId, code, CancellationToken.None));
    }

    [Fact]
    public async Task ConfirmEmailAsync_NullUserIdAndInvalidCode_ReturnsFalse()
    {
        // Arrange
        string userId = null;
        var code = "123";
        var tenant = "root";
        _userServiceMock.Setup(x => x.ConfirmEmailAsync(userId, code, tenant, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InternalServerException("An error occurred while confirming E-Mail."));

        // Act & Assert
        await Assert.ThrowsAsync<InternalServerException>(() =>
            _controller.ConfirmEmailAsync(tenant, userId, code, CancellationToken.None));
    }

    [Fact]
    public async Task ConfirmEmailAsync_ValidUserIdAndNullCode_ReturnsFalse()
    {
        // Arrange
        var userId = "5b15f43e-41a9-4ce7-a86f-dd646c22079e";
        string code = null;
        var tenant = "root";
        _userServiceMock.Setup(x => x.ConfirmEmailAsync(userId, code, tenant, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InternalServerException("An error occurred while confirming E-Mail."));

        // Act & Assert
        await Assert.ThrowsAsync<InternalServerException>(() =>
            _controller.ConfirmEmailAsync(tenant, userId, code, CancellationToken.None));
    }

    [Fact]
    public async Task ConfirmEmailAsync_InvalidUserIdAndNullCode_ReturnsFalse()
    {
        // Arrange
        var userId = "123";
        string code = null;
        var tenant = "root";
        _userServiceMock.Setup(x => x.ConfirmEmailAsync(userId, code, tenant, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InternalServerException("An error occurred while confirming E-Mail."));

        // Act & Assert
        await Assert.ThrowsAsync<InternalServerException>(() =>
            _controller.ConfirmEmailAsync(tenant, userId, code, CancellationToken.None));
    }

    // Test for the ConfrimPhoneNumberForUser

    [Fact]
    public async Task UTC01_ConfirmPhoneNumberAsync_ValidUserIdAndCode_ReturnsTrue()
    {
        // Arrange
        var userId = "5b15f43e-41a9-4ce7-a86f-dd646c22079e";
        var code = "32bc02d0-3641-4c82-9e55-977ac12caab8";
        SetupUserContext(userId, true);
        _userServiceMock.Setup(x => x.ConfirmPhoneNumberAsync(userId, code))
            .ReturnsAsync("success");

        // Act
        var result = await _controller.ConfirmPhoneNumberAsync(code);

        // Assert
        Assert.Equal("success", result);
    }

    [Fact]
    public async Task UTC02_ConfirmPhoneNumberAsync_ValidUserIdInvalidCode_ReturnsFalse()
    {
        // Arrange
        var userId = "5b15f43e-41a9-4ce7-a86f-dd646c22079e";
        var code = "123";
        SetupUserContext(userId, true);
        _userServiceMock.Setup(x => x.ConfirmPhoneNumberAsync(userId, code))
            .ThrowsAsync(new InternalServerException("Failure"));

        // Act & Assert
        await Assert.ThrowsAsync<InternalServerException>(() =>
            _controller.ConfirmPhoneNumberAsync(code));
    }

    [Fact]
    public async Task UTC03_ConfirmPhoneNumberAsync_ValidUserIdNullCode_ReturnsFalse()
    {
        // Arrange
        var userId = "5b15f43e-41a9-4ce7-a86f-dd646c22079e";
        string code = null;
        SetupUserContext(userId, true);
        _userServiceMock.Setup(x => x.ConfirmPhoneNumberAsync(userId, code))
            .ThrowsAsync(new InternalServerException("Failure"));

        // Act & Assert
        await Assert.ThrowsAsync<InternalServerException>(() =>
            _controller.ConfirmPhoneNumberAsync(code));
    }

    [Fact]
    public async Task UTC04_ConfirmPhoneNumberAsync_InvalidUserIdValidCode_ReturnsFalse()
    {
        // Arrange
        var userId = "6f0fdd25-6888-4ed2-b5c8-baf9019797cc";
        var code = "32bc02d0-3641-4c82-9e55-977ac12caab8";
        SetupUserContext(userId, true);
        _userServiceMock.Setup(x => x.ConfirmPhoneNumberAsync(userId, code))
            .ThrowsAsync(new InternalServerException("Failure"));

        // Act & Assert
        await Assert.ThrowsAsync<InternalServerException>(() =>
            _controller.ConfirmPhoneNumberAsync(code));
    }

    [Fact]
    public async Task UTC05_ConfirmPhoneNumberAsync_InvalidUserIdInvalidCode_ReturnsFalse()
    {
        // Arrange
        var userId = "6f0fdd25-6888-4ed2-b5c8-baf9019797cc";
        var code = "123";
        SetupUserContext(userId, true);
        _userServiceMock.Setup(x => x.ConfirmPhoneNumberAsync(userId, code))
            .ThrowsAsync(new InternalServerException("Failure"));

        // Act & Assert
        await Assert.ThrowsAsync<InternalServerException>(() =>
            _controller.ConfirmPhoneNumberAsync(code));
    }

    [Fact]
    public async Task UTC06_ConfirmPhoneNumberAsync_InvalidUserIdNullCode_ReturnsFalse()
    {
        // Arrange
        var userId = "6f0fdd25-6888-4ed2-b5c8-baf9019797cc";
        string code = null;
        SetupUserContext(userId, true);
        _userServiceMock.Setup(x => x.ConfirmPhoneNumberAsync(userId, code))
            .ThrowsAsync(new InternalServerException("Failure"));

        // Act & Assert
        await Assert.ThrowsAsync<InternalServerException>(() =>
            _controller.ConfirmPhoneNumberAsync(code));
    }

    [Fact]
    public async Task UTC07_ConfirmPhoneNumberAsync_NullUserIdValidCode_ReturnsFalse()
    {
        // Arrange
        string userId = null;
        var code = "32bc02d0-3641-4c82-9e55-977ac12caab8";
        SetupUserContext(userId, true);
        _userServiceMock.Setup(x => x.ConfirmPhoneNumberAsync(userId, code))
            .ThrowsAsync(new InternalServerException("Failure"));

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _controller.ConfirmPhoneNumberAsync(code));
    }

    [Fact]
    public async Task UTC08_ConfirmPhoneNumberAsync_NullUserIdInvalidCode_ReturnsFalse()
    {
        // Arrange
        string userId = null;
        var code = "123";
        SetupUserContext(userId, true);
        _userServiceMock.Setup(x => x.ConfirmPhoneNumberAsync(userId, code))
            .ThrowsAsync(new InternalServerException("Failure"));

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _controller.ConfirmPhoneNumberAsync(code));
    }

    [Fact]
    public async Task UTC09_ConfirmPhoneNumberAsync_NullUserIdNullCode_ReturnsFalse()
    {
        // Arrange
        string userId = null;
        string code = null;
        SetupUserContext(userId, true);
        _userServiceMock.Setup(x => x.ConfirmPhoneNumberAsync(userId, code))
            .ThrowsAsync(new InternalServerException("Failure"));

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _controller.ConfirmPhoneNumberAsync(code));
    }

    // ResendPhoneNumberConfirmationCode

    [Fact]
    public async Task UTC01_ResendPhoneNumberCodeConfirmAsync_Success()
    {
        // Arrange
        var userId = "5b15f43e-41a9-4ce7-a86f-dd646c22079e";
        SetupUserContext(userId, true);
        _mediatorMock.Setup(m => m.Send(It.IsAny<ResendPhoneCodeRequest>(), default))
        .ReturnsAsync("Send code successfully!");

        // Act
        var result = await _controller.ResendPhoneNumberCodeConfirmAsync();

        // Assert
        Assert.Equal("Send code successfully!", result);
    }

    [Fact]
    public async Task UTC02_ResendPhoneNumberCodeConfirmAsync_UserNotAuthenticated()
    {
        // Arrange
        string userId = "6f0fdd25-6888-4ed2-b5c8-baf9019797cc";
        SetupUserContext(userId, false);
        _mediatorMock.Setup(m => m.Send(It.IsAny<ResendPhoneCodeRequest>(), default))
        .ThrowsAsync(new UnauthorizedAccessException(_t["User is not authenticated."]));

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _controller.ResendPhoneNumberCodeConfirmAsync());
    }

    [Fact]
    public async Task UTC03_ResendPhoneNumberCodeConfirmAsync_UserAlreadyVerified()
    {
        // Arrange
        var userId = "5b15f43e-41a9-4ce7-a86f-dd646c22079e";
        SetupUserContext(userId, true);
        _mediatorMock.Setup(m => m.Send(It.IsAny<ResendPhoneCodeRequest>(), default))
            .ThrowsAsync(new InternalServerException(_t["An error occurred while resending Mobile Phone confirmation code."]));

        // Act & Assert
        await Assert.ThrowsAsync<InternalServerException>(() =>
            _controller.ResendPhoneNumberCodeConfirmAsync());
    }

    [Fact]
    public async Task UTC04_ResendPhoneNumberCodeConfirmAsync_ServerError()
    {
        // Arrange
        SetupUserContext("5b15f43e-41a9-4ce7-a86f-dd646c22079e", false);
        _mediatorMock.Setup(m => m.Send(It.IsAny<ResendPhoneCodeRequest>(), default))
            .ThrowsAsync(new InternalServerException("An error occurred while resending Mobile Phone confirmation code."));

        // Act & Assert
        await Assert.ThrowsAsync<InternalServerException>(() =>
            _controller.ResendPhoneNumberCodeConfirmAsync());
    }

    // ResendEmailAddressConfirmationCode
    [Fact]
    public async Task UTC01_ResendEmailConfirmAsync_Success()
    {
        // Arrange
        var userId = "5b15f43e-41a9-4ce7-a86f-dd646c22079e";
        SetupUserContext(userId, true);
        _mediatorMock.Setup(m => m.Send(It.IsAny<ResendEmailConfirmRequest>(), default))
        .ReturnsAsync("Send code successfully!");

        // Act
        var result = await _controller.ResendEmailConfirmAsync();

        // Assert
        Assert.Equal("Send code successfully!", result);
    }

    [Fact]
    public async Task UTC02_ResendEmailConfirmAsync_UserNotAuthenticated()
    {
        // Arrange
        string userId = "6f0fdd25-6888-4ed2-b5c8-baf9019797cc";
        SetupUserContext(userId, false);
        _mediatorMock.Setup(m => m.Send(It.IsAny<ResendEmailConfirmRequest>(), default))
        .ThrowsAsync(new UnauthorizedAccessException(_t["User is not authenticated."]));

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _controller.ResendEmailConfirmAsync());
    }

    [Fact]
    public async Task UTC03_ResendEmailConfirmAsync_UserAlreadyVerified()
    {
        // Arrange
        var userId = "5b15f43e-41a9-4ce7-a86f-dd646c22079e";
        SetupUserContext(userId, true);
        _mediatorMock.Setup(m => m.Send(It.IsAny<ResendEmailConfirmRequest>(), default))
            .ThrowsAsync(new InternalServerException(_t["An error occurred while resending Mobile Phone confirmation code."]));

        // Act & Assert
        await Assert.ThrowsAsync<InternalServerException>(() =>
            _controller.ResendEmailConfirmAsync());
    }

    [Fact]
    public async Task UTC04_ResendEmailConfirmAsync_ServerError()
    {
        // Arrange
        SetupUserContext("5b15f43e-41a9-4ce7-a86f-dd646c22079e", false);
        _mediatorMock.Setup(m => m.Send(It.IsAny<ResendEmailConfirmRequest>(), default))
            .ThrowsAsync(new InternalServerException("An error occurred while resending Mobile Phone confirmation code."));

        // Act & Assert
        await Assert.ThrowsAsync<InternalServerException>(() =>
            _controller.ResendEmailConfirmAsync());
    }
}
