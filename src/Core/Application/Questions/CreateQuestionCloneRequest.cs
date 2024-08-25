using FSH.WebApi.Application.Common.Interfaces;
using FSH.WebApi.Application.Questions.Dtos;
using FSH.WebApi.Application.Questions.Specs;
using FSH.WebApi.Domain.Examination;
using FSH.WebApi.Domain.Question;
using Mapster;

namespace FSH.WebApi.Application.Questions;
public class CreateQuestionCloneRequest : IRequest<Guid>
{
    public required Guid OriginalQuestionId { get; set; }
}

public class CreateQuestionCloneRequestValidator : CustomValidator<CreateQuestionCloneRequest>
{
    public CreateQuestionCloneRequestValidator()
    {

    }
}

public class CreateQuestionCloneRequestHandler : IRequestHandler<CreateQuestionCloneRequest, Guid>
{
    private readonly IRepositoryWithEvents<QuestionClone> _questionCloneRepo;
    private readonly IRepositoryWithEvents<Question> _questionRepo;
    private readonly IStringLocalizer _t;
    private readonly ICurrentUser _currentUser;
    private readonly IRepository<QuestionFolder> _questionFolderRepository;

    public CreateQuestionCloneRequestHandler(
               IRepositoryWithEvents<QuestionClone> questionCloneRepo,
               IRepositoryWithEvents<Question> questionRepo,
                      IStringLocalizer<CreateQuestionCloneRequestHandler> t,
                      ICurrentUser currentUser,
                      IRepository<QuestionFolder> questionFolderRepository)
    {
        _questionCloneRepo = questionCloneRepo;
        _t = t;
        _currentUser = currentUser;
        _questionFolderRepository = questionFolderRepository;
        _questionRepo = questionRepo;
    }

    public async Task<Guid> Handle(CreateQuestionCloneRequest request, CancellationToken cancellationToken)
    {

        var existingQuestion = await _questionRepo.FirstOrDefaultAsync(new Questions.Specs.QuestionByIdSpec(request.OriginalQuestionId));
        if (existingQuestion == null)
            throw new NotFoundException(_t["Question {0} Not Found.", request.OriginalQuestionId]);

        var questionCloneDto = existingQuestion.Adapt<CreateQuestionCloneDto>();
        questionCloneDto.QuestionLabelId = existingQuestion.QuestionLableId;
        questionCloneDto.OriginalQuestionId = existingQuestion.Id;

        var questionClone = questionCloneDto.Adapt<QuestionClone>();
        questionClone.QuestionLabelId = existingQuestion.QuestionLableId;
        await _questionCloneRepo.AddAsync(questionClone,cancellationToken);

        return questionClone.Id;
    }

  
}

