using FSH.WebApi.Application.Examination.Papers;
using FSH.WebApi.Application.Examination.PaperStudents.Dtos;
using FSH.WebApi.Application.Examination.PaperStudents.Specs;
using FSH.WebApi.Domain.Examination;
using Mapster;

namespace FSH.WebApi.Application.Examination.PaperStudents;
public class GetHistoryTestOfStudentRequest : PaginationFilter, IRequest<PaginationResponse<StudentTestHistoryDto>>
{
}

public class GetHistoryTestOfStudentRequestHandler
    : IRequestHandler<GetHistoryTestOfStudentRequest, PaginationResponse<StudentTestHistoryDto>>
{
    private readonly ICurrentUser _currentUser;
    private readonly IReadRepository<SubmitPaper> _submitPaperRepo;
    private readonly IRepository<Paper> _paperRepo;

    public GetHistoryTestOfStudentRequestHandler(
        ICurrentUser currentUser,
        IReadRepository<SubmitPaper> submitPaperRepo,
        IRepository<Paper> paperRepo)
    {
        _currentUser = currentUser;
        _submitPaperRepo = submitPaperRepo;
        _paperRepo = paperRepo;
    }

    public async Task<PaginationResponse<StudentTestHistoryDto>> Handle(
        GetHistoryTestOfStudentRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.GetUserId();
        var spec = new HistorySubmitPaperSpec(request, userId);
        var submitions = await _submitPaperRepo.ListAsync(spec, cancellationToken);

        var data = new List<StudentTestHistoryDto>();
        foreach (var item in submitions)
        {
            var paper = await _paperRepo.FirstOrDefaultAsync(new PaperByIdSpec(item.PaperId), cancellationToken);

            item.TotalMark = item.getScore(paper.SubmitPapers.Count);
            var submitHistory = item.Adapt<StudentTestHistoryDto>();
            submitHistory.CanViewDetailAnswer = item.CheckDetailAnswerResult(paper.SubmitPapers.Count);
            data.Add(submitHistory);
        }

        return new PaginationResponse<StudentTestHistoryDto>
            (data, await _submitPaperRepo.CountAsync(spec, cancellationToken), request.PageNumber, request.PageSize);
    }
}
