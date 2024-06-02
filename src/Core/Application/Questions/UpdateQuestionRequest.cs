
using FSH.WebApi.Application.Questions.Specs;
using FSH.WebApi.Domain.Question;
using FSH.WebApi.Domain.Question.Enums;
using Mapster;


namespace FSH.WebApi.Application.Questions;
public class UpdateQuestionRequest : IRequest<Guid>
{
    public Guid Id { get; set; }
    public string? Content { get; set; }
    public string? Image { get; set; }
    public string? Audio { get; set; }
    public Guid? QuestionFolderId { get; set; }
    public QuestionType? QuestionType { get; set; }
    public Guid? QuestionLabelId { get; set; }
    public Guid? ParentId { get; set; }
    public List<AnswerDto>? Answers { get; set; }
}



public class UpdateQuestionRequestValidator : CustomValidator<UpdateQuestionRequest>
{
    public UpdateQuestionRequestValidator(
        IReadRepository<QuestionFolder> _folderRepo,
        IReadRepository<QuestionLable> _labelRepo,
        IStringLocalizer<UpdateQuestionRequestValidator> _t)
    {
        RuleFor(x => x.QuestionFolderId)
            .MustAsync(async (folderId, ct) => !folderId.HasValue || await _folderRepo.FirstOrDefaultAsync(new QuestionFolderByIdSpec(folderId), ct) is not null)
            .WithMessage((_, folderId) => _t["Folder {0} Not Found.", folderId]);
        RuleFor(x => x.QuestionLabelId)
            .MustAsync(async (labelId, ct) => !labelId.HasValue || await _labelRepo.FirstOrDefaultAsync(new QuestionLabelByIdSpec(labelId), ct) is not null)
            .WithMessage((_, labelId) => _t["Question Label {0} Not Found.", labelId]);
    }
}

public class UpdateQuestionRequestHandler : IRequestHandler<UpdateQuestionRequest, Guid>
{
    private readonly IRepositoryWithEvents<Question> _questionRepo;
    private readonly IStringLocalizer _t;
    private readonly ICurrentUser _currentUser;
    private readonly IRepositoryWithEvents<Answer> _answerRepo;

    public UpdateQuestionRequestHandler(
        IRepositoryWithEvents<Question> questionRepo,
        IStringLocalizer<UpdateQuestionRequestHandler> t,
        ICurrentUser currentUser,
        IRepositoryWithEvents<Answer> answerRepo)
    {
        _questionRepo = questionRepo;
        _t = t;
        _currentUser = currentUser;
        _answerRepo = answerRepo;
    }

    public async Task<DefaultIdType> Handle(UpdateQuestionRequest request, CancellationToken cancellationToken)
    {
        var question = await _questionRepo.FirstOrDefaultAsync(new QuestionByIdSpec(request.Id));
        _ = question ?? throw new NotFoundException(_t["Question {0} Not Found.", request.Id]);

        if (!question.CanUpdate(_currentUser.GetUserId()))
            throw new ForbiddenException(_t["You can not edit."]);

        question.Update(request.Content, request.Image, request.Audio, request.QuestionFolderId, request.QuestionType, request.QuestionLabelId, request.ParentId);

        if (request.Answers != null)
        {
            var answers = request.Answers.Adapt<List<Answer>>();
            question.UpdateAnswers(answers);
        }

        await _questionRepo.UpdateAsync(question);

        return question.Id;
    }


}