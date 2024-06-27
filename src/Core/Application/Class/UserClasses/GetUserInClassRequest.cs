using FSH.WebApi.Application.Class.New;
using FSH.WebApi.Application.Class.UserClasses.Dto;
using FSH.WebApi.Application.Common.Persistence;
using FSH.WebApi.Application.Multitenancy;
using FSH.WebApi.Application.TeacherGroup.GroupTeachers;
using FSH.WebApi.Domain.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class.UserClasses;
public class GetUserInClassRequest : IRequest<List<UserClass>>
{
    public Guid ClassId { get; set; }

    public GetUserInClassRequest(Guid classId) => ClassId = classId;
}

public class GetUserInClassRequestHandler : IRequestHandler<GetUserInClassRequest, List<UserClass>>
{
    private readonly IUserClassesRepository _userClassesRepository;
    public GetUserInClassRequestHandler(IUserClassesRepository userClassesRepository)
    {
        _userClassesRepository = userClassesRepository;
    }

    public async Task<List<UserClass>> Handle(GetUserInClassRequest request, CancellationToken cancellationToken)
            => await _userClassesRepository.GetUserInClasses(request.ClassId);
}
