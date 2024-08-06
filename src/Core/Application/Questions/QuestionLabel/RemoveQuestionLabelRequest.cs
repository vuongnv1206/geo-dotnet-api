using FSH.WebApi.Domain.Question;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Questions.QuestionLabel;
public class RemoveQuestionLabelRequest : IRequest<Guid>
{
    public Guid Id { get; set; }
    public RemoveQuestionLabelRequest(Guid id)
    {
        Id = id;
    }
}

public class RemoveQuestionLabelRequestHandler : IRequestHandler<RemoveQuestionLabelRequest, Guid>
{
    private readonly IRepositoryWithEvents<QuestionLable> _repo;
    private readonly IStringLocalizer _t;

    public RemoveQuestionLabelRequestHandler(
        IRepositoryWithEvents<QuestionLable> repo,
        IStringLocalizer<RemoveQuestionLabelRequestHandler> t)
    {
        _repo = repo;
        _t = t;
    }

    public async Task<DefaultIdType> Handle(RemoveQuestionLabelRequest request, CancellationToken cancellationToken)
    {
        var label = await _repo.GetByIdAsync(request.Id, cancellationToken);
        _ = label ?? throw new NotFoundException(_t["Label {0} Not Found.", request.Id]);

        await _repo.DeleteAsync(label, cancellationToken);

        return label.Id;
    }
}
