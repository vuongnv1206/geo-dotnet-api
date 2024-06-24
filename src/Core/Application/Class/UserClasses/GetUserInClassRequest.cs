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
public class GetUserInClassRequest : IRequest<List<UserClassDto>>
{
    public Guid ClassId { get; set; }

    public GetUserInClassRequest(Guid classId) => ClassId = classId;
}

public class GetUserInClassRequestHandler : IRequestHandler<GetUserInClassRequest, List<UserClassDto>>
{
    private readonly IUserClassesRepository _userClassesRepository;
    public GetUserInClassRequestHandler(IUserClassesRepository userClassesRepository)
    {
        _userClassesRepository = userClassesRepository;
    }

    public async Task<List<UserClassDto>> Handle(GetUserInClassRequest request, CancellationToken cancellationToken)
    {
        return default;
        //var data = await (request.ClassId, cancellationToken);
        //var data= await _repository.FirstOrDefaultAsync(new GroupTeacherByIdSpec(request.Id), cancellationToken);

        //if (groupTeacher == null)
        //    throw new NotFoundException(_t["GroupTeacher{0} Not Found.", request.Id]);

        //return groupTeacher.Adapt<GroupTeacherDto>();
    }
}
