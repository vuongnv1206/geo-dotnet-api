using FSH.WebApi.Application.Assignments.Dtos;
using FSH.WebApi.Domain.Assignment;

namespace FSH.WebApi.Application.Assignments;

public class GetAssignmentViaDapperRequest : IRequest<AssignmentDto>
{
    public Guid Id { get; set; }

    public GetAssignmentViaDapperRequest(Guid id) => Id = id;
}

public class GetAssignmentViaDapperRequestHandler : IRequestHandler<GetAssignmentViaDapperRequest, AssignmentDto>
{
    private readonly IDapperRepository _repository;
    private readonly IStringLocalizer _t;

    public GetAssignmentViaDapperRequestHandler(IDapperRepository repository, IStringLocalizer<GetAssignmentViaDapperRequestHandler> localizer) =>
        (_repository, _t) = (repository, localizer);

    public async Task<AssignmentDto> Handle(GetAssignmentViaDapperRequest request, CancellationToken cancellationToken)
    {
        var assignment = await _repository.QueryFirstOrDefaultAsync<Assignment>(
            $"SELECT * FROM \"Assignment\".\"Assignment\" WHERE \"Id\"  = '{request.Id}' AND \"TenantId\" = '@tenant'", cancellationToken: cancellationToken);

        _ = assignment ?? throw new NotFoundException(_t["Assignment {0} Not Found.", request.Id]);

        // Using mapster here throws a nullreference exception because of the "SubjectName" property
        // in AssignmentDto and the assignment not having a Brand assigned.
        return new AssignmentDto
        {
            Id = assignment.Id,
            Name = assignment.Name,
            StartTime = assignment.StartTime,
            EndTime = assignment.EndTime,
            AttachmentPath = assignment.AttachmentPath,
            Content = assignment.Content,
            CanViewResult = assignment.CanViewResult,
            RequireLoginToSubmit = assignment.RequireLoginToSubmit,
            SubjectId = (DefaultIdType)assignment.SubjectId,
            SubjectName = string.Empty
        };
    }
}