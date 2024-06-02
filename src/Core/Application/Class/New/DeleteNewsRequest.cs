using FSH.WebApi.Application.Class.New.Spec;
using FSH.WebApi.Domain.Catalog;
using FSH.WebApi.Domain.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class.New;
public class DeleteNewsRequest : IRequest<Guid>
{
    public Guid Id { get; set; }

    public DeleteNewsRequest(Guid id) => Id = id;
}

public class DeleteNewsRequestHandler : IRequestHandler<DeleteNewsRequest, Guid>
{
    private readonly IRepository<News> _repository;

    private readonly IStringLocalizer _t;

    public DeleteNewsRequestHandler(IRepository<News> repository,IStringLocalizer<DeleteNewsRequestHandler> t)
    {
        _repository = repository;
        _t = t;
    }

    public async Task<DefaultIdType> Handle(DeleteNewsRequest request, CancellationToken cancellationToken)
    {
        var news = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = news ?? throw new NotFoundException(_t["News {0} Not Found."]);

        var newsComment = await _repository.ListAsync(new NewsCommentByParentIdSpec(request.Id), cancellationToken);
        if (newsComment != null)
        {
            await _repository.DeleteRangeAsync(newsComment,cancellationToken);
        }

        await _repository.DeleteAsync(news, cancellationToken);

        return request.Id;
    }
}
