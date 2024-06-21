using FSH.WebApi.Application.Examination.Papers;
using FSH.WebApi.Application.Extensions;
using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.SubmitPapers;
public class UpdateSubmitPaperRequest : IRequest<Guid>
{
    public Guid Id { get; set; }
    public SubmitPaperStatus Status { get; set; } = SubmitPaperStatus.End;
}

public class UpdateSubmitPaperRequestVAlidator : CustomValidator<UpdateSubmitPaperRequest>
{
    public UpdateSubmitPaperRequestVAlidator(
        IRepository<Paper> paperRepository,
        IRepository<SubmitPaper> submitPaperRepo,
        IStringLocalizer<CreateSubmitPaperRequestValidator> T)
    {
        RuleFor(x => x.Id)
            .MustAsync(async (submitId, ct) => await submitPaperRepo.FirstOrDefaultAsync(new SubmitPaperByIdSpec(submitId), ct) is not null)
            .WithMessage((_, submitId) => T["SubmitPaper {0} Not Found.", submitId]);
    }
}

public class UpdateSubmitPaperRequestHandler : IRequestHandler<UpdateSubmitPaperRequest, Guid>
{
    private readonly IRepository<SubmitPaper> _submitPaperRepo;
    private readonly ICurrentUser _currentUser;
    private readonly IStringLocalizer _t;

    public UpdateSubmitPaperRequestHandler(
        IRepository<SubmitPaper> submitPaperRepo,
        ICurrentUser currentUser,
        IStringLocalizer<UpdateSubmitPaperRequestHandler> t)
    {
        _submitPaperRepo = submitPaperRepo;
        _currentUser = currentUser;
        _t = t;
    }

    public async Task<DefaultIdType> Handle(UpdateSubmitPaperRequest request, CancellationToken cancellationToken)
    {
        var submitPaper = await _submitPaperRepo.FirstOrDefaultAsync(new SubmitPaperByIdSpec(request.Id), cancellationToken)
            ?? throw new NotFoundException(_t["Submit Paper {0} Not Found.", request.Id]);

        var userId = _currentUser.GetUserId();
        if (submitPaper.Paper.CreatedBy != userId && submitPaper.CreatedBy != userId)
        {
            throw new ForbiddenException(_t["Can't update this submit."]);
        }

        if (request.Status == SubmitPaperStatus.End
            && submitPaper.Status == SubmitPaperStatus.Start)
        {
            throw new ConflictException(_t["The Paper {0} has ever done.", submitPaper.Paper.Id]);
        }

        if (request.Status == SubmitPaperStatus.End)
        {
            float totalMark = 0;

            foreach (var submit in submitPaper.SubmitPaperDetails)
            {
                float markOfQuestion = 0;
                if (submit.Question.QuestionParentId is null
                    || submit.Question.QuestionParentId == Guid.Empty)
                {
                    markOfQuestion = submit.GetPointQuestion(submit.Question,
                        submitPaper.Paper.PaperQuestions.FirstOrDefault(x => x.QuestionId == submit.QuestionId).Mark);
                }
                else
                {
                    var paperQuestionParent = submitPaper.Paper.PaperQuestions
                   .FirstOrDefault(x => x.QuestionId == submit.Question.QuestionParentId);

                    float avgMark = paperQuestionParent.Mark / paperQuestionParent.Question.QuestionPassages.Count;

                    markOfQuestion = submit.GetPointQuestion(submit.Question, avgMark);
                }

                submit.Mark = markOfQuestion;
                totalMark += markOfQuestion;
            }

            submitPaper.TotalMark = totalMark;
        }
        else
        {
            submitPaper.TotalMark = 0;
        }


        submitPaper.Status = request.Status;
        await _submitPaperRepo.UpdateAsync(submitPaper);

        return submitPaper.Id;
    }
}