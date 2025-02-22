﻿using FSH.WebApi.Domain.Class;

namespace FSH.WebApi.Application.Class.New;
public class CreatePostRequest : IRequest<Guid>
{
    public Guid ClassesId { get; set; }
    public string? Content { get; set; }
    public bool IsLockComment { get; set; }
}

public class CreateNewsRequestValidator : CustomValidator<CreatePostRequest>
{
    public CreateNewsRequestValidator(
        IReadRepository<Classes> classRepos,
        IStringLocalizer<CreateNewsRequestValidator> T)
    {
        RuleFor(p => p.ClassesId)
            .NotEmpty()
            .MustAsync(async (id, ct) => await classRepos.GetByIdAsync(id, ct) is not null)
            .WithMessage((_, id) => T["Classes {0} Not Found.", id]);
    }
}

public class CreateNewsRequestHandler : IRequestHandler<CreatePostRequest, Guid>
{
    private readonly IRepositoryWithEvents<Post> _repository;
    private readonly ICurrentUser _currentUser;
    private readonly IRepository<Classes> _classRepo;
    private readonly IStringLocalizer _t;

    public CreateNewsRequestHandler(
        IRepositoryWithEvents<Post> repository,
        ICurrentUser currentUser,
        IRepository<Classes> classRepo,
        IStringLocalizer<CreateNewsRequestHandler> t)
    {
        _repository = repository;
        _currentUser = currentUser;
        _classRepo = classRepo;
        _t = t;
    }

    public async Task<Guid> Handle(CreatePostRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.GetUserId();

        _ = await _classRepo.FirstOrDefaultAsync(new ClassByIdSpec(request.ClassesId, userId), cancellationToken)
            ?? throw new NotFoundException(_t["Class {0} Not Found", request.ClassesId]);

        var post = new Post(request.Content ?? string.Empty, request.IsLockComment, request.ClassesId);

        await _repository.AddAsync(post);

        return post.Id;
    }
}
