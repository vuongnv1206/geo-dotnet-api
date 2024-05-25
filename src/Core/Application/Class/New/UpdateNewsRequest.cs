using FSH.WebApi.Domain.Class;
using FSH.WebApi.Domain.Common.Events;

namespace FSH.WebApi.Application.Class.New;
public class UpdateNewsRequest : IRequest<Guid>
{
    public Guid Id { get; set; }
    public required string Content { get; set; }
    public bool IsLockComment { get; set; }
    public Guid? ParentId { get; set; }
}

public class UpdateNewsRequestHandler : IRequestHandler<UpdateNewsRequest, Guid>
{

    public readonly IRepository<News> _repository;
    private readonly IStringLocalizer _t;

    public UpdateNewsRequestHandler(IRepository<News> repository, IStringLocalizer<UpdateNewsRequestHandler> localizer) =>
        (_repository, _t) = (repository, localizer);

    public async Task<DefaultIdType> Handle(UpdateNewsRequest request, CancellationToken cancellationToken)
    {
        var news = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = news ?? throw new NotFoundException(_t["News {0} Not Found.", request.Id]);

        var updateNews = news.Update(request.Content, request.IsLockComment, request.ParentId);

        news.DomainEvents.Add(EntityUpdatedEvent.WithEntity(news));

        await _repository.UpdateAsync(updateNews, cancellationToken);

        return request.Id;

    }
}
