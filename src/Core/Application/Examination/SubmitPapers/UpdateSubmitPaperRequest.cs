using FSH.WebApi.Application.Examination.Papers;
using FSH.WebApi.Application.Extensions;
using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.SubmitPapers;
public class UpdateSubmitPaperRequest : IRequest<Guid>
{
    public Guid Id { get; set; }
    public Guid PaperId { get; set; }
    public SubmitPaperStatus Status { get; set; } = SubmitPaperStatus.End;
}

public class UpdateSubmitPaperRequestVAlidator : CustomValidator<UpdateSubmitPaperRequest>
{
    public UpdateSubmitPaperRequestVAlidator(
        IRepository<Paper> paperRepository,
        IRepository<SubmitPaper> submitPaperRepo,
        IStringLocalizer<CreateSubmitPaperRequestValidator> T)
    {
        RuleFor(x => x.PaperId)
            .MustAsync(async (paperId, ct) => await paperRepository.FirstOrDefaultAsync(new PaperByIdSpec(paperId), ct) is not null)
            .WithMessage((_, paperId) => T["Paper {0} Not Found.", paperId]);

        RuleFor(x => x.Id)
            .MustAsync(async (submitId, ct) => await submitPaperRepo.FirstOrDefaultAsync(new SubmitPaperByIdSpec(submitId), ct) is not null)
            .WithMessage((_, submitId) => T["SubmitPaper {0} Not Found.", submitId]);
    }
}

public class UpdateSubmitPaperRequestHandler : IRequestHandler<UpdateSubmitPaperRequest, Guid>
{
    private readonly IRepository<Paper> _paperRepo;
    private readonly ICurrentUser _currentUser;
    private readonly IStringLocalizer _t;

    public UpdateSubmitPaperRequestHandler(
        IRepository<Paper> paperRepo,
        ICurrentUser currentUser,
        IStringLocalizer<UpdateSubmitPaperRequestHandler> t)
    {
        _paperRepo = paperRepo;
        _currentUser = currentUser;
        _t = t;
    }

    public async Task<DefaultIdType> Handle(UpdateSubmitPaperRequest request, CancellationToken cancellationToken)
    {
        var paper = await _paperRepo.FirstOrDefaultAsync(new PaperByIdSpec(request.PaperId), cancellationToken)
            ?? throw new NotFoundException(_t["Paper {0} Not Found.", request.PaperId]);

        var submitPaper = paper.SubmitPapers.FirstOrDefault(x => x.Id == request.Id)
            ?? throw new NotFoundException(_t["Submit Paper {0} Not Found.", request.Id]);

        var userId = _currentUser.GetUserId();
        if (paper.CreatedBy != userId && submitPaper.CreatedBy != userId)
        {
            throw new ForbiddenException(_t["Can't update this submit."]);
        }

        if (request.Status == SubmitPaperStatus.End
            && submitPaper.Status == SubmitPaperStatus.Start)
        {
            throw new ConflictException(_t["The Paper {0} has ever done.", request.PaperId]);
        }

        if (request.Status == SubmitPaperStatus.End)
        {
            float totalMark = 0;

            submitPaper.SubmitPaperDetails.ForEach(submit =>
            {
                totalMark += submit.GetPointQuestion(paper.PaperQuestions.FirstOrDefault(x => x.QuestionId == submit.QuestionId));
            });

            submitPaper.TotalMark = totalMark;
        } else {
            submitPaper.TotalMark = 0;
        }


        submitPaper.Status = request.Status;
        await _paperRepo.UpdateAsync(paper);

        return submitPaper.Id;
    }
}