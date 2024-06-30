using FSH.WebApi.Domain.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class.New;
public class CreatePostRequest : IRequest<Guid>
{
    public Guid ClassesId { get; set; }
    public Guid UserId { get; set; }

    public string? Content { get; set; }
    public bool IsLockComment { get; set; }
}

public class CreateNewsRequestValidator : CustomValidator<CreatePostRequest>
{
    public CreateNewsRequestValidator(IReadRepository<Classes> classRepos, IStringLocalizer<CreateNewsRequestValidator> T)
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

    public CreateNewsRequestHandler(IRepositoryWithEvents<Post> repository, ICurrentUser currentUser) =>
        (_repository, _currentUser) = (repository, currentUser);

    public async Task<Guid> Handle(CreatePostRequest request, CancellationToken cancellationToken)
    {
        var post = new Post(request.Content ?? string.Empty, request.IsLockComment, request.UserId, request.ClassesId);

        await _repository.AddAsync(post);

        return post.Id;
    }
}
