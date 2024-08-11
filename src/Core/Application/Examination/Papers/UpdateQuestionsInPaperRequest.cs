

using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.Papers;
public class UpdateQuestionsInPaperRequest : IRequest
{
    public Guid PaperId { get; set; }
    public List<CreateUpdateQuestionInPaperDto>? Questions { get; set; }
}


public class UpdateQuestionsInPaperRequestHandler : IRequestHandler<UpdateQuestionsInPaperRequest>
{
    private readonly IRepository<Paper> _paperRepository;
    private readonly IStringLocalizer<UpdateQuestionsInPaperRequestHandler> _t;
    private readonly IMediator _mediator;

    public UpdateQuestionsInPaperRequestHandler(
        IRepository<Paper> paperRepository,
        IStringLocalizer<UpdateQuestionsInPaperRequestHandler> t,
        IMediator mediator)
    {
        _paperRepository = paperRepository;
        _t = t;
        _mediator = mediator;
    }

    public async Task<Unit> Handle(UpdateQuestionsInPaperRequest request, CancellationToken cancellationToken)
    {   
        var paper = await _paperRepository.FirstOrDefaultAsync(new PaperByIdSpec(request.PaperId), cancellationToken);
        if (paper == null)
        {
            throw new NotFoundException($"Paper with Id {request.PaperId} not found.");
        }

        var existingQuestionIds = paper.PaperQuestions.Select(q => q.Question.OriginalQuestionId).ToList();

        var questionsToAdd = new List<CreateUpdateQuestionInPaperDto>();
        var questionsToUpdate = new List<CreateUpdateQuestionInPaperDto>();
        var questionIdsToDelete = new List<Guid?>();

        foreach (var questionDto in request.Questions)
        {
            if (existingQuestionIds.Contains(questionDto.QuestionId))
            {
                questionsToUpdate.Add(questionDto);
            }
            else
            {
                questionsToAdd.Add(questionDto);
            }
        }

        // Tìm các câu hỏi cần xóa
        questionIdsToDelete = existingQuestionIds
            .Where(id => request.Questions.All(q => q.QuestionId != id))
            .ToList();

        if (questionIdsToDelete.Any())
        {
            foreach (var questionId in questionIdsToDelete)
            {
                var deleteRequest = new DeleteQuestionInPaperRequest
                {
                    PaperId = request.PaperId,
                    QuestionCloneId = questionId.Value
                };
                await _mediator.Send(deleteRequest, cancellationToken);
            }
        }

        // Gọi các request tương ứng
        if (questionsToAdd.Any())
        {
            var addRequest = new AddQuestionsInPaperRequest
            {
                PaperId = request.PaperId,
                Questions = questionsToAdd
            };
            await _mediator.Send(addRequest, cancellationToken);
        }

       

        // Cập nhật các câu hỏi cần update
        foreach (var questionDto in questionsToUpdate)
        {
            var questionToUpdate = paper.PaperQuestions.FirstOrDefault(q => q.Question.OriginalQuestionId == questionDto.QuestionId);
            if (questionToUpdate != null)
            {
                questionToUpdate.Mark = questionDto.Mark;
                questionToUpdate.RawIndex = questionDto.RawIndex;
            }
        }

        await _paperRepository.UpdateAsync(paper, cancellationToken);

        return Unit.Value;
    }
}
