using FSH.WebApi.Domain.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class.New;
public class CreateNewsRequest : IRequest<Guid>
{
    public Guid ClassesId { get; set; }
    public string? Content { get; set; }
    public bool IsLockCommnet { get; set; }
    public Guid? ParentId { get; set; }
}

public class CreateNewsRequestValidator : CustomValidator<CreateNewsRequest>
{
    public CreateNewsRequestValidator(IReadRepository<Classes> classRepos, IStringLocalizer<CreateNewsRequestValidator> T)
    {
        RuleFor(p => p.ClassesId)
            .NotEmpty()
            .MustAsync(async (id, ct) => await classRepos.GetByIdAsync(id, ct) is not null)
            .WithMessage((_, id) => T["Classes {0} Not Found.", id]);
    }
}

public class CreateNewsRequestHandler : IRequestHandler<CreateNewsRequest, Guid>
{
    private readonly IRepositoryWithEvents<News> _repository;
    private readonly ICurrentUser _currentUser;

    public CreateNewsRequestHandler(IRepositoryWithEvents<News> repository, ICurrentUser currentUser) =>
        (_repository, _currentUser) = (repository, currentUser);

    public async Task<Guid> Handle(CreateNewsRequest request, CancellationToken cancellationToken)
    {
        var news = new News(request.Content, request.IsLockCommnet, request.ParentId, request.ClassesId);

        await _repository.AddAsync(news);

        return news.Id;
    }
}
