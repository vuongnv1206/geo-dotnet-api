using FSH.WebApi.Application.Class.UserStudents.Dto;
using FSH.WebApi.Domain.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class.UserStudents.Spec;
public class UserStudentBySearchSpec : EntitiesByPaginationFilterSpec<UserStudent, UserStudentDto>
{
    public UserStudentBySearchSpec(SearchUserStudentRequest request, Guid currentUserId)
        : base(request)
    {
        Query.OrderBy(c => c.CreatedOn, !request.HasOrderBy())
            .Where(x => x.CreatedBy == currentUserId);
    }
}
