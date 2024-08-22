

using FSH.WebApi.Application.Examination.SubmitPapers.Dtos;
using FSH.WebApi.Domain.Examination;
using FSH.WebApi.Domain.Question;

namespace FSH.WebApi.Application.Examination.SubmitPapers;
public class MarkAnswerRequest : IRequest<Guid>
{
    public Guid SubmitPaperId { get; set; }
    public Guid QuestionId { get; set; }
    public float Mark { get; set; }
}

public class MarkAnswerRequestValidator : CustomValidator<MarkAnswerRequest>
{
    public MarkAnswerRequestValidator(
        IRepository<SubmitPaper> submitPaperRepo,
        IRepository<QuestionClone> questionRepo,
        IStringLocalizer<MarkAnswerRequestValidator> T)
    {
        RuleFor(x => x.SubmitPaperId)
            .MustAsync(async (submitPaperId, ct) => await submitPaperRepo.GetByIdAsync(submitPaperId, ct) is not null)
                .WithMessage((_, submitPaperId) => T["Submit Paper {0} Not Found", submitPaperId]);
        RuleFor(x => x.QuestionId)
            .MustAsync(async (questionId, ct) => await questionRepo.GetByIdAsync(questionId, ct) is not null)
                .WithMessage((_, questionId) => T["Question {0} Not Found", questionId]);
    }
}

public class MarkAnswerRequestHandler : IRequestHandler<MarkAnswerRequest, Guid>
{
    private readonly IRepository<SubmitPaper> _submitPaperRepo;
    private readonly IRepository<Question> _questionRepo;
    private readonly IRepository<QuestionClone> _questionCloneRepo;
    private readonly IStringLocalizer _t;
    private readonly ISerializerService _serializerService;
    public MarkAnswerRequestHandler(
        IRepository<SubmitPaper> submitPaperRepo,
        IStringLocalizer<MarkAnswerRequestHandler> t,
        IRepository<Question> questionRepo,
        ISerializerService serializerService,
        IRepository<QuestionClone> questionCloneRepo)
    {
        _submitPaperRepo = submitPaperRepo;
        _t = t;
        _questionRepo = questionRepo;
        _serializerService = serializerService;
        _questionCloneRepo = questionCloneRepo;
    }

    public async Task<DefaultIdType> Handle(MarkAnswerRequest request, CancellationToken cancellationToken)
    {
        var submitPaper = await _submitPaperRepo.FirstOrDefaultAsync(new SubmitPaperByIdSpec(request.SubmitPaperId));
        if (submitPaper is null)
            throw new NotFoundException(_t["Submit Paper {0} Not Found.", request.SubmitPaperId]);

        // Kiểm tra trạng thái của SubmitPaper
        if (submitPaper.Status != SubmitPaperStatus.End)
            throw new ConflictException(_t["Cannot mark this submit paper. This test is not over yet.", request.SubmitPaperId]);

        var question = await _questionCloneRepo.GetByIdAsync(request.QuestionId);
        if (question is null)
            throw new NotFoundException(_t["Question {0} Not Found.", request.QuestionId]);
        if (question.QuestionType != Domain.Question.Enums.QuestionType.Writing)
            throw new ConflictException(_t["Cannot mark this question. Question ID: {0} is not of type 'Writing'.", request.QuestionId]);
        var answer = submitPaper.SubmitPaperDetails
                .FirstOrDefault(x => x.SubmitPaperId == request.SubmitPaperId
                                                   && x.QuestionId == request.QuestionId);
        if (answer is null)
            throw new NotFoundException(_t["Answer for this question not found."]);

        submitPaper.MarkAnswer(answer, request.Mark);

        await _submitPaperRepo.UpdateAsync(submitPaper);
        return submitPaper.Id;

    }
}

