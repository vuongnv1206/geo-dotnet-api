using FSH.WebApi.Application.Common.Interfaces;
using FSH.WebApi.Application.Common.Persistence;
using FSH.WebApi.Application.Examination.SubmitPapers;
using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Domain.Examination;
using FSH.WebApi.Infrastructure.Examination;
using Microsoft.Extensions.Localization;
namespace Infrastructure.Test.Examination;
public class SubmmitPaperServiceTests
{
    private readonly ISubmmitPaperService? _submmitPaperService;

    // Create mock objects for the above fields
    private readonly Moq.Mock<IRepository<Paper>> _paperRepository = new();
    private readonly Moq.Mock<IRepository<SubmitPaper>> _submitPaperRepository = new();
    private readonly Moq.Mock<ICurrentUser> _currentUser = new();
    private readonly Moq.Mock<IUserService> _userService = new();
    private readonly Moq.Mock<IStringLocalizer<SubmmitPaperService>> _t = new();
    private readonly Moq.Mock<ISerializerService> _serializerService = new();

}
