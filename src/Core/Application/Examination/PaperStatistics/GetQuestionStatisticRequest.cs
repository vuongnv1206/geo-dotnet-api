using FSH.WebApi.Application.Common.Interfaces;
using FSH.WebApi.Application.Examination.PaperFolders;
using FSH.WebApi.Application.Examination.Papers;
using FSH.WebApi.Application.Examination.SubmitPapers;
using FSH.WebApi.Application.Questions;
using FSH.WebApi.Domain.Examination;
using FSH.WebApi.Domain.Question;
using Mapster;

namespace FSH.WebApi.Application.Examination.PaperStatistics;
public class GetQuestionStatisticRequest : PaginationFilter, IRequest<PaginationResponse<QuestionStatisticDto>>
{
    public Guid PaperId { get; set; }
    public Guid? ClassId { get; set; }
}

public class GetQuestionStatisticRequestHandler : IRequestHandler<GetQuestionStatisticRequest, PaginationResponse<QuestionStatisticDto>>
{
    private readonly IRepository<Paper> _paperRepo;
    private readonly IStringLocalizer _t;
    private readonly IRepository<QuestionClone> _questionCloneRepo;
    private readonly IRepository<SubmitPaperDetail> _submitDetailRepo;
    private readonly ISubmmitPaperService _submmitPaperService;

    public GetQuestionStatisticRequestHandler(
        IRepository<Paper> paperRepo,
        IStringLocalizer<GetQuestionStatisticRequestHandler> t,
        IRepository<QuestionClone> questionCloneRepo,
        IRepository<SubmitPaperDetail> submitDetailRepo,
        ISubmmitPaperService submmitPaperService)
    {
        _paperRepo = paperRepo;
        _t = t;
        _questionCloneRepo = questionCloneRepo;
        _submitDetailRepo = submitDetailRepo;
        _submmitPaperService = submmitPaperService;
    }

    public async Task<PaginationResponse<QuestionStatisticDto>> Handle(GetQuestionStatisticRequest request, CancellationToken cancellationToken)
    {
        var paper = await _paperRepo.FirstOrDefaultAsync(new PaperByIdSpec(request.PaperId))
            ?? throw new NotFoundException(_t["Paper {0} Not Found.", request.PaperId]);

        var questionsInPaper = await _questionCloneRepo.ListAsync(
            new QuestionCloneInPaperSpec(request, paper.PaperQuestions.Select(x => x.QuestionId).ToList()));

        var response = questionsInPaper.Select(x =>
        {
            var dto = x.Adapt<QuestionStatisticDto>();
            return dto;
        }).ToList();

        foreach(var dto in response)
        {
            var question = await _questionCloneRepo.FirstOrDefaultAsync(new QuestionCloneByIdSpec(dto.Id));
            if (question.QuestionType == Domain.Question.Enums.QuestionType.Reading)
            {
                foreach (var passage in dto.QuestionPassages)
                {
                    var submitDetails = await _submitDetailRepo.ListAsync(new SubmitPaperDetailByQuestionId(passage.Id, request.PaperId));
                    passage.TotalAnswered = submitDetails.Count;
                    passage.TotalTest = paper.SubmitPapers.Count;
                    var questionPassage = await _questionCloneRepo.FirstOrDefaultAsync(new QuestionCloneByIdSpec(passage.Id));
                    foreach (var sd in submitDetails)
                    {
                        if (_submmitPaperService.IsCorrectAnswer(sd, questionPassage))
                        {
                            passage.TotalCorrect++;
                        }
                        else
                        {
                            passage.TotalWrong++;
                        }
                    }
                }
            }
            else
            {
                var submitDetails = await _submitDetailRepo.ListAsync(new SubmitPaperDetailByQuestionId(question.Id, request.PaperId));
                dto.TotalAnswered = submitDetails.Count;
                dto.TotalTest = paper.SubmitPapers.Count;
                foreach (var sd in submitDetails)
                {
                    if (_submmitPaperService.IsCorrectAnswer(sd, question))
                    {
                        dto.TotalCorrect++;
                    }
                    else
                    {
                        dto.TotalWrong++;
                    }
                }
            }
        }

        return new PaginationResponse<QuestionStatisticDto>(response, response.Count, request.PageNumber, request.PageSize);
    }
}
