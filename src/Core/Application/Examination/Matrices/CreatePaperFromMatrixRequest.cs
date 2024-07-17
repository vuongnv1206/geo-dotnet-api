
using FSH.WebApi.Application.Examination.Papers;
using FSH.WebApi.Application.Questions.Specs;
using FSH.WebApi.Domain.Examination;
using FSH.WebApi.Domain.Examination.Enums;
using FSH.WebApi.Domain.Question;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Examination.Matrices;
public class CreatePaperFromMatrixRequest : IRequest<Guid>
{
    public Guid MatrixId { get; set; }
    public string ExamName { get; set; }
    public PaperStatus Status { get; set; }
    public string? Password { get; set; }
    public PaperType Type { get; set; }
    public string? Content { get; set; }
    public string? Description { get; set; }
    public Guid? PaperLabelId { get; set; }
    public Guid? PaperFolderId { get; set; }
    public Guid? SubjectId { get; set; }
}

public class CreatePaperFromMatrixRequestHandler : IRequestHandler<CreatePaperFromMatrixRequest, Guid>
{
    private readonly IRepositoryWithEvents<Paper> _paperRepo;
    private readonly IRepository<PaperMatrix> _matrixRepo;
    private readonly IStringLocalizer<CreatePaperFromMatrixRequestHandler> _t;
    private readonly IMediator _mediator;
    private readonly IReadRepository<Domain.Question.Question> _questionRepo;
    private readonly IReadRepository<QuestionFolder> _questionFolderRepo;
    public CreatePaperFromMatrixRequestHandler(
        IRepositoryWithEvents<Paper> paperRepo,
        IRepository<PaperMatrix> matrixRepo,
        IStringLocalizer<CreatePaperFromMatrixRequestHandler> t,
        IMediator mediator,
        IReadRepository<Domain.Question.Question> questionRepo,
        IReadRepository<QuestionFolder> questionFolderRepo
        )
    {
        _paperRepo = paperRepo;
        _matrixRepo = matrixRepo;
        _t = t;
        _mediator = mediator;
        _questionRepo = questionRepo;
        _questionFolderRepo = questionFolderRepo;
    }

    public async Task<Guid> Handle(CreatePaperFromMatrixRequest request, CancellationToken cancellationToken)
    {
        var matrix = await _matrixRepo.GetByIdAsync(request.MatrixId);
        _ = matrix ?? throw new NotFoundException(_t["Matrix {0} Not Found.", request.MatrixId]);

        var matrixContent = JsonConvert.DeserializeObject<List<ContentMatrixDto>>(matrix.Content);

        var createPaperRequest = new CreatePaperRequest(request.ExamName, request.Status, request.Password, request.Type, request.Content, request.Description, request.PaperLabelId, request.PaperFolderId, request.SubjectId);

        foreach (var item in matrixContent)
        {
            var rootQuestionFolder = await _questionFolderRepo.FirstOrDefaultAsync(new QuestionFolderByIdSpec(item.QuestionFolderId));
            var folderIds = new List<Guid>();
            folderIds.Add(item.QuestionFolderId);
            rootQuestionFolder.ChildQuestionFolderIds(rootQuestionFolder.Children, folderIds);

            int totalNumberOfQuestions = matrixContent.Sum(item => item.CriteriaQuestions.Sum(criteria => criteria.NumberOfQuestion));
            float markPerQuestion = item.TotalPoint / totalNumberOfQuestions;

            foreach (var criteria in item.CriteriaQuestions)
            {
                var spec = new QuestionFromMatrixSpec(folderIds, criteria.QuestionLabelId, criteria.QuestionType);
                var questions = await _questionRepo.ListAsync(spec, cancellationToken);

                if (questions.Count < criteria.NumberOfQuestion)
                {
                    throw new ValidationException($"Not enough questions found for {criteria.QuestionLabelId}.");
                }

                var selectedQuestions = questions.OrderBy(x => Guid.NewGuid()).Take(criteria.NumberOfQuestion).ToList();
                var rawIndexes = criteria.RawIndex.Split(',').Select(int.Parse).ToList();
                for (int i = 0; i < selectedQuestions.Count; i++)
                {
                    createPaperRequest.Questions.Add(new CreateUpdateQuestionInPaperDto
                    {
                        QuestionId = selectedQuestions[i].Id,
                        Mark = markPerQuestion,
                        RawIndex = rawIndexes.Count > i ? rawIndexes[i] : 0
                    });
                }
            }
        }

        return await _mediator.Send(createPaperRequest);
    }
}
