using FSH.WebApi.Application.Class.New.Spec;
using FSH.WebApi.Domain.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class;
public class DeleteClassRequest : IRequest<Guid>
{
    public Guid Id { get; set; }
    public DeleteClassRequest(Guid id) => Id = id;
}

public class DeleteClassRequestHandler : IRequestHandler<DeleteClassRequest, Guid>
{
    private readonly IRepositoryWithEvents<Classes> _repository;
    private readonly IStringLocalizer<DeleteClassRequest> _t;
    private readonly IRepositoryWithEvents<News> _newRepository;

    public DeleteClassRequestHandler(IRepositoryWithEvents<Classes> repository, IStringLocalizer<DeleteClassRequest> t, IRepositoryWithEvents<News> newRepository)
    {
        _repository = repository;
        _t = t;
        _newRepository = newRepository;
    }

    public async Task<Guid> Handle(DeleteClassRequest request, CancellationToken cancellationToken)
    {
        var classes = await _repository.FirstOrDefaultAsync(new ClassByIdSpec(request.Id), cancellationToken);
        _ = classes ?? throw new NotFoundException(_t["Classes {0} Not Found."]);

        var news = await _newRepository.ListAsync(new NewByClassIdSpec(request.Id), cancellationToken);
        await _newRepository.DeleteRangeAsync(news, cancellationToken);

        await _repository.DeleteAsync(classes, cancellationToken);

        return request.Id;
    }
}
