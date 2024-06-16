using FSH.WebApi.Application.Extensions;
using FSH.WebApi.Application.Questions.Specs;
using FSH.WebApi.Domain.Examination;
using FSH.WebApi.Domain.Question;
using FSH.WebApi.Domain.Question.Enums;
using System.Text.RegularExpressions;

namespace FSH.WebApi.Application.Examination.SubmitPapers;
public class SubmitAnswerRawRequest : IRequest<Guid>
{
    public Guid SubmitPaperId { get; set; }
    public Guid QuestionId { get; set; }
    public string? AnswerRaw { get; set; }
}

public class SubmitAnswerRawRequestValidator : CustomValidator<SubmitAnswerRawRequest>
{
    public SubmitAnswerRawRequestValidator(
        IRepository<SubmitPaper> submitPaperRepo,
        IRepository<Question> questionRepo,
        IStringLocalizer<SubmitAnswerRawRequestValidator> T)
    {
        RuleFor(x => x.SubmitPaperId)
            .MustAsync(async (submitPaperId, ct) => await submitPaperRepo.GetByIdAsync(submitPaperId, ct) is not null)
                .WithMessage((_, submitPaperId) => T["Submit Paper {0} Not Found", submitPaperId]);
        RuleFor(x => x.QuestionId)
            .MustAsync(async (questionId, ct) => await questionRepo.GetByIdAsync(questionId, ct) is not null)
                .WithMessage((_, questionId) => T["Question {0} Not Found", questionId]);
    }
}

public class SubmitAnswerRawRequestHandler : IRequestHandler<SubmitAnswerRawRequest, Guid>
{
    private readonly IRepository<SubmitPaper> _submitPaperRepo;
    private readonly IRepository<Question> _questionRepo;
    private readonly IStringLocalizer _t;
    private readonly ISerializerService _serializerService;
    public SubmitAnswerRawRequestHandler(
        IRepository<SubmitPaper> submitPaperRepo,
        IStringLocalizer<SubmitAnswerRawRequestHandler> t,
        IRepository<Question> questionRepo,
        ISerializerService serializerService)
    {
        _submitPaperRepo = submitPaperRepo;
        _t = t;
        _questionRepo = questionRepo;
        _serializerService = serializerService;
    }

    public async Task<DefaultIdType> Handle(SubmitAnswerRawRequest request, CancellationToken cancellationToken)
    {
        var submitPaper = await _submitPaperRepo.FirstOrDefaultAsync(new SubmitPaperByIdSpec(request.SubmitPaperId));
        if (submitPaper is null)
            throw new NotFoundException(_t["Submit Paper {0} Not Found.", request.SubmitPaperId]);

        if (submitPaper.Status == SubmitPaperStatus.End)
        {
            throw new ConflictException(_t["This {0} paper is over"]);
        }
        else
        {
            submitPaper.Status = SubmitPaperStatus.Doing;
        }

        var question = await _questionRepo.FirstOrDefaultAsync(new QuestionByIdSpec(request.QuestionId));
        if (question is null)
            throw new NotFoundException(_t["Question {0} Not Found.", request.QuestionId]);

        string answerRaw = FormatAnswerRaw(request.AnswerRaw, question);

        submitPaper.SubmitAnswerRaw(new SubmitPaperDetail(
                request.SubmitPaperId,
                request.QuestionId,
                answerRaw));

        await _submitPaperRepo.UpdateAsync(submitPaper);

        return submitPaper.Id;
    }

    private string FormatAnswerRaw(string? answerRaw, Question question)
    {
        // SingleChoice : answerId
        // MultipleChoise : answerId|answerId
        // FillBlank : [{
        //                            "1" : "content"
        //                          }
        //              ]
        // Matching : 1_2|2_1|4_3|3_4
        // Writing : raw

        if (string.IsNullOrEmpty(answerRaw))
            return string.Empty;

        switch (question.QuestionType)
        {
            case QuestionType.SingleChoice:
            case QuestionType.ReadingQuestionPassage:
                if (!question.Answers.Any(x => x.Id.ToString() == answerRaw))
                    throw new NotFoundException(_t["Answer {0} Not Found.", answerRaw]);

                return answerRaw;
            case QuestionType.MultipleChoice:
                var answerList = answerRaw.SplitStringExtension("|");
                foreach (string ans in answerList)
                {
                    if (!question.Answers.Any(x => x.Id.ToString() == ans))
                    {
                        throw new NotFoundException(_t["Answer {0} Not Found.", ans]);
                    }
                }

                return answerRaw;
            case QuestionType.Matching:
                string pattern = @"^\d+_\d+(\|\d+_\d+)*$";
                if (!Regex.IsMatch(answerRaw, pattern))
                    throw new ConflictException(_t["Wrong format answer submit: number_number|number_number"]);

                return answerRaw;
            case QuestionType.FillBlank:
                var answers = _serializerService.Deserialize<List<Dictionary<string, string>>>(answerRaw);

                return answerRaw;
            case QuestionType.Writing:
                return answerRaw;
            default:
                return string.Empty;
        }
    }
}
