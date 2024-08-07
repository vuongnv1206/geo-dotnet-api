using FSH.WebApi.Application.Questions.Dtos;
using FSH.WebApi.Application.Questions.Specs;
using FSH.WebApi.Domain.Question;
using FSH.WebApi.Domain.Question.Enums;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Questions;
public class GetQuestionRandomRequest : IRequest<List<QuestionDto>>
{
    public List<Guid> FolderIds { get; set; }
    public QuestionType QuestionType { get; set; }
    public Guid QuestionLabelId { get; set; }
    public int NumberQuestion { get; set; }

    public GetQuestionRandomRequest(
        List<DefaultIdType> folderIds,
        QuestionType questionType,
        DefaultIdType questionLabelId,
        int numberQuestion)
    {
        FolderIds = folderIds;
        QuestionType = questionType;
        QuestionLabelId = questionLabelId;
        NumberQuestion = numberQuestion;
    }
}

public class GetQuestionRandomRequestHandler : IRequestHandler<GetQuestionRandomRequest, List<QuestionDto>>
{
    readonly IReadRepository<Domain.Question.Question> _questionRepo;

    public GetQuestionRandomRequestHandler(
        IReadRepository<Domain.Question.Question> questionRepo)
    {
        _questionRepo = questionRepo;
    }

    public async Task<List<QuestionDto>> Handle(GetQuestionRandomRequest request, CancellationToken cancellationToken)
    {
        var spec = new QuestionFromMatrixSpec(request.FolderIds, request.QuestionLabelId, request.QuestionType);
        var questions = await _questionRepo.ListAsync(spec, cancellationToken);

        if (questions.Count < request.NumberQuestion)
        {
            throw new BadRequestException("Not enough questions found.");
        }

        var random = new Random();
        var questionRandom = questions.OrderBy(x => random.Next()).Take(request.NumberQuestion).ToList();

        return questionRandom.Adapt<List<QuestionDto>>();
    }
}
