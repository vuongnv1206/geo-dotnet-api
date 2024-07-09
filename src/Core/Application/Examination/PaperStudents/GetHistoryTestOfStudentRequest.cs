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
    private readonly IRepository<SubmitPaper> _submitPaperRepo;

    public GetHistoryTestOfStudentRequestHandler(
        ICurrentUser currentUser,
        IRepository<SubmitPaper> submitPaperRepo)
    {
        _currentUser = currentUser;
        _submitPaperRepo = submitPaperRepo;
    }

    public async Task<PaginationResponse<StudentTestHistoryDto>> Handle
        (GetHistoryTestOfStudentRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.GetUserId();

        var spec = new HistorySubmitPaperSpec(request, userId);
        var submittedPaper = await _submitPaperRepo.ListAsync(spec, cancellationToken);

        foreach(var sp in submittedPaper)
        {
            if (sp.Paper.ShowMarkResult == ShowResult.No)
            {
                sp.TotalMark = 0;
            }
        }

        var res = submittedPaper.Adapt<List<StudentTestHistoryDto>>();
        return new PaginationResponse<StudentTestHistoryDto>(res, submittedPaper.Count, request.PageNumber, request.PageSize);
    }
}
