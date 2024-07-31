using FSH.WebApi.Application.Class;
using FSH.WebApi.Application.Examination.Papers;
using FSH.WebApi.Application.Examination.SubmitPapers;
using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Application.Questions;
using FSH.WebApi.Domain.Class;
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
    private readonly IUserService _userService;
    private readonly IRepository<Classes> _classRepo;
    private readonly ICurrentUser _currentUser;

    public GetQuestionStatisticRequestHandler(
        IRepository<Paper> paperRepo,
        IStringLocalizer<GetQuestionStatisticRequestHandler> t,
        IRepository<QuestionClone> questionCloneRepo,
        IRepository<SubmitPaperDetail> submitDetailRepo,
        ISubmmitPaperService submmitPaperService,
        IUserService userService,
        IRepository<Classes> classRepo,
        ICurrentUser currentUser)
    {
        _paperRepo = paperRepo;
        _t = t;
        _questionCloneRepo = questionCloneRepo;
        _submitDetailRepo = submitDetailRepo;
        _submmitPaperService = submmitPaperService;
        _userService = userService;
        _classRepo = classRepo;
        _currentUser = currentUser;
    }

    public async Task<PaginationResponse<QuestionStatisticDto>> Handle(GetQuestionStatisticRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.GetUserId();

        var paper = await _paperRepo.FirstOrDefaultAsync(new PaperByIdSpec(request.PaperId))
            ?? throw new NotFoundException(_t["Paper {0} Not Found.", request.PaperId]);

        var questionIds = paper.PaperQuestions.Select(x => x.QuestionId).ToList();
        var questionsInPaper = await _questionCloneRepo.ListAsync(new QuestionCloneInPaperSpec(request, questionIds));

        var response = questionsInPaper.Adapt<List<QuestionStatisticDto>>();

        foreach (var dto in response)
        {
            var question = await _questionCloneRepo.FirstOrDefaultAsync(new QuestionCloneByIdSpec(dto.Id));
            var classroom = request.ClassId.HasValue
                ? await _classRepo.FirstOrDefaultAsync(new ClassByIdSpec(request.ClassId.Value, userId))
                : null;
            dto.RawIndex = paper.PaperQuestions.FirstOrDefault(x => x.QuestionId == dto.Id).RawIndex;

            int totalTest = paper.SubmitPapers.Count(x => classroom == null
            || classroom.UserClasses.Any(uc => uc.Student.StId == x.CreatedBy));

            if (question.QuestionType == Domain.Question.Enums.QuestionType.Reading)
            {
                foreach (var passage in dto.QuestionPassages)
                {
                    var submitDetails = await _submitDetailRepo.ListAsync(new SubmitPaperDetailByQuestionId(passage.Id, request.PaperId));

                    passage.TotalAnswered = submitDetails.Count(x => classroom == null
                                                                    || classroom.UserClasses.Any(uc => uc.Student.StId == x.CreatedBy));
                    passage.TotalTest = totalTest;

                    var questionPassage = await _questionCloneRepo.FirstOrDefaultAsync(new QuestionCloneByIdSpec(passage.Id))
                        ?? throw new NotFoundException(_t["Question {0} Not Found.", passage.Id]);

                    foreach (var sd in submitDetails.Where(x => classroom == null
                                                                    || classroom.UserClasses.Any(uc => uc.Student.StId == x.CreatedBy)))
                    {
                        if (_submmitPaperService.IsCorrectAnswer(sd, questionPassage))
                        {
                            passage.TotalCorrect++;
                        }
                        else
                        {
                            passage.TotalWrong++;
                            var studentInfo = await _userService.GetAsync(sd.CreatedBy.ToString(), cancellationToken);
                            passage.WrongStudents.Add(new StudentInfo
                            {
                                Id = sd.CreatedBy,
                                FirstName = studentInfo.FirstName,
                                LastName = studentInfo.LastName,
                            });
                        }
                    }
                }
            }
            else
            {
                var submitDetails = await _submitDetailRepo.ListAsync(new SubmitPaperDetailByQuestionId(question.Id, request.PaperId));

                dto.TotalAnswered = submitDetails.Count(x => classroom == null
                                                         || classroom.UserClasses.Any(uc => uc.Student.StId == x.CreatedBy));
                dto.TotalTest = totalTest;

                foreach (var sd in submitDetails.Where(x => classroom == null
                                                                    || classroom.UserClasses.Any(uc => uc.Student.StId == x.CreatedBy)))
                {
                    if (_submmitPaperService.IsCorrectAnswer(sd, question))
                    {
                        dto.TotalCorrect++;
                    }
                    else
                    {
                        dto.TotalWrong++;
                        var studentInfo = await _userService.GetAsync(sd.CreatedBy.ToString(), cancellationToken);
                        dto.WrongStudents.Add(new StudentInfo
                        {
                            Id = sd.CreatedBy,
                            FirstName = studentInfo.FirstName,
                            LastName = studentInfo.LastName,
                        });
                    }
                }
            }
        }

        return new PaginationResponse<QuestionStatisticDto>(response, response.Count, request.PageNumber, request.PageSize);
    }
}
