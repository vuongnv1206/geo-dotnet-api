namespace FSH.WebApi.Application.Identity.Users.Profile;
public class UpdateAvatarRequest : IRequest<string>
{
    public string? UserId { get; set; }
    public FileUploadRequest? Image { get; set; }
    public bool DeleteCurrentImage { get; set; } = false;
}

public class UpdateAvatarRequestHandler : IRequestHandler<UpdateAvatarRequest, string>
{
    private readonly IUserService _userService;
    private readonly IStringLocalizer<UpdateEmailRequestHandler> _t;

    public UpdateAvatarRequestHandler(IUserService userManager, IStringLocalizer<UpdateEmailRequestHandler> t)
    {
        _userService = userManager;
        _t = t;
    }

    public async Task<string> Handle(UpdateAvatarRequest request, CancellationToken cancellationToken)
    {
        await _userService.UpdateAvatarAsync(request);
        return _t["Avatar updated successfully."];
    }
}

