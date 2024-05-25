using FSH.WebApi.Application.Questions.Specs;
using FSH.WebApi.Domain.Question;
using FSH.WebApi.Domain.Question.Enums;

namespace FSH.WebApi.Application.Questions;
public class UpdateAQuestionRequest : IRequest<Guid>
{
    public required UpdateQuestionDto Question { get; set; }
}

public class UpdateQuestionDto
{
    public Guid Id { get; set; }
    public string? Content { get; set; }
    public string? Image { get; set; }
    public string? Audio { get; set; }
    public Guid? QuestionFolderId { get; set; }
    public QuestionType? QuestionType { get; set; }
    public Guid? QuestionLableId { get; set; }
    public Guid? ParentId { get; set; }
    public List<UpdateQuestionDto>? QuestionPassages { get; set; }
    public List<AnswerDto>? Answers { get; set; }
}

public class UpdateAQuestionRequestValidator : CustomValidator<UpdateAQuestionRequest>
{
    public UpdateAQuestionRequestValidator(
        IReadRepository<QuestionFolder> FolderRepo,
        IReadRepository<QuestionLable> LabelRepo,
        IStringLocalizer<UpdateAQuestionRequestValidator> T)
    {
        RuleFor(x => x.Question.QuestionFolderId)
            .MustAsync(async (folderId, ct) => !folderId.HasValue || await FolderRepo.FirstOrDefaultAsync(new QuestionFolderByIdSpec(folderId), ct) is not null)
            .WithMessage((_, folderId) => T["Folder {0} Not Found.", folderId]);
        RuleFor(x => x.Question.QuestionLableId)
            .MustAsync(async (labelId, ct) => !labelId.HasValue || await LabelRepo.FirstOrDefaultAsync(new QuestionLabelByIdSpec(labelId), ct) is not null)
            .WithMessage((_, labelId) => T["Question Label {0} Not Found.", labelId]);
    }
}

public class UpdateAQuestionRequestHandler : IRequestHandler<UpdateAQuestionRequest, Guid>
{
    private readonly IRepositoryWithEvents<Question> _questionRepo;
    private readonly IStringLocalizer _t;
    private readonly ICurrentUser _currentUser;
    private readonly IRepositoryWithEvents<Answer> _answerRepo;

    public UpdateAQuestionRequestHandler(
        IRepositoryWithEvents<Question> questionRepo,
        IStringLocalizer<UpdateAQuestionRequestHandler> t,
        ICurrentUser currentUser,
        IRepositoryWithEvents<Answer> answerRepo)
    {
        _questionRepo = questionRepo;
        _t = t;
        _currentUser = currentUser;
        _answerRepo = answerRepo;
    }

    public async Task<DefaultIdType> Handle(UpdateAQuestionRequest request, CancellationToken cancellationToken)
    {
        var question = await _questionRepo.FirstOrDefaultAsync(new QuestionByIdRequestSpec(request.Question.Id));
        if (question is null)
            throw new NotFoundException(_t["Question {0} Not Found.", request.Question.Id]);
        if (!question.CanUpdate(_currentUser.GetUserId()))
            throw new ForbiddenException(_t["You can not edit."]);

        question.Update(
            request.Question.Content,
            request.Question.Image,
            request.Question.Audio,
            request.Question.QuestionFolderId,
            request.Question.QuestionType,
            request.Question.QuestionLableId,
            request.Question.ParentId);

        await UpdateAnswers(question.Answers, request.Question.Answers, question);

        await _questionRepo.UpdateAsync(question);

        return question.Id;
    }

    private async Task UpdateAnswers(List<Answer>? answers, List<AnswerDto>? answerRequest, Question question)
    {
        foreach (var answerExist in answers)
        {
            var matchingAnswer = answerRequest.FirstOrDefault(x => x.Id == answerExist.Id);
            if (matchingAnswer is null)
            {
                await _answerRepo.DeleteAsync(answerExist);
            }
            else
            {
                answerExist.Update(matchingAnswer.Content, matchingAnswer.IsCorrect);
                await _answerRepo.UpdateAsync(answerExist);
                answerRequest.Remove(matchingAnswer);
            }
        }

        foreach (var newAnwer in answerRequest)
        {
            var answer = new Answer(newAnwer.Content, newAnwer.QuestionId, newAnwer.IsCorrect);

            question.AddAnswer(answer);
        }
    }
}
