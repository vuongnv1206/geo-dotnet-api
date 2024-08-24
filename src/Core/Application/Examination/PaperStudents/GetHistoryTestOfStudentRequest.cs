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

    public GetHistoryTestOfStudentRequestHandler(
        ICurrentUser currentUser,
        IReadRepository<SubmitPaper> submitPaperRepo)
    {
        _currentUser = currentUser;
        _submitPaperRepo = submitPaperRepo;
    }

    public async Task<PaginationResponse<StudentTestHistoryDto>> Handle(
        GetHistoryTestOfStudentRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.GetUserId();
        var spec = new HistorySubmitPaperSpec(request, userId);
        var submitions = await _submitPaperRepo.ListAsync(spec, cancellationToken);

        var data = submitions.Adapt<List<StudentTestHistoryDto>>();

        return new PaginationResponse<StudentTestHistoryDto>
            (data, await _submitPaperRepo.CountAsync(spec, cancellationToken), request.PageNumber, request.PageSize);
    }
}
