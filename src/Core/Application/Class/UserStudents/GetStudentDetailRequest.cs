using FSH.WebApi.Application.Class.UserStudents.Dto;
using FSH.WebApi.Domain.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSH.WebApi.Application.Class.UserStudents.Spec;

namespace FSH.WebApi.Application.Class.UserStudents;
public class GetStudentDetailRequest : IRequest<UserStudentDto>
{
    public Guid Id { get; set; }

    public GetStudentDetailRequest(Guid id) => Id = id;
}

public class GetStudentDetailRequestHandler : IRequestHandler<GetStudentDetailRequest, UserStudentDto>
{
    private readonly IRepository<Student> _repository;
    private readonly IStringLocalizer _t;

    public GetStudentDetailRequestHandler(IRepository<Student> repository, IStringLocalizer<GetStudentDetailRequestHandler> localizer) =>
        (_repository, _t) = (repository, localizer);

    public async Task<UserStudentDto> Handle(GetStudentDetailRequest request, CancellationToken cancellationToken) =>
        await _repository.FirstOrDefaultAsync(
            (ISpecification<Student, UserStudentDto>)new StudentDetailByIdSpec(request.Id), cancellationToken)
        ?? throw new NotFoundException(_t["Student {0} Not Found.", request.Id]);
}