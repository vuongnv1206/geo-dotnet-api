using FSH.WebApi.Application.Common.Specification;
using FSH.WebApi.Domain.Class;
using FSH.WebApi.Domain.Examination.Enums;

namespace FSH.WebApi.Application.Examination.PaperAccesses;
public class GroupClassAccessPaperSpec : EntitiesByPaginationFilterSpec<GroupClass>
{
    // lấy tất cả paperaccess car classId và userId
    public GroupClassAccessPaperSpec(List<Guid> classIds, List<Guid> studentIds, GetGroupClassesAccessPaperRequest request, Guid userId)
        : base(request)
    {
       Query.Include(x => x.Classes)
        .ThenInclude(c => c.UserClasses)
        .ThenInclude(c => c.Student)
        .Where(x => x.CreatedBy == userId &&  x.Classes.Any(c => classIds.Contains(c.Id) || c.UserClasses.Any(uc => studentIds.Contains(uc.StudentId))));

    }

    // lấy theo loại class hay user
    public GroupClassAccessPaperSpec(List<Guid> accesserIds, GetGroupClassesAccessPaperRequest request, Guid userId, PaperShareType status)
      : base(request)
    {
        if (status == PaperShareType.AssignToStudent)
        {
            Query.Include(x => x.Classes)
             .ThenInclude(c => c.UserClasses)
             .ThenInclude(c => c.Student)
             .Where(x => x.CreatedBy == userId && x.Classes.Any(c => c.UserClasses.Any(uc => accesserIds.Contains(uc.StudentId))));

        }

        if (status == PaperShareType.AssignToClass)
        {
            Query.Include(x => x.Classes)
             .ThenInclude(c => c.UserClasses)
             .ThenInclude(c => c.Student)
             .Where(x => x.CreatedBy == userId && x.Classes.Any(c => accesserIds.Contains(c.Id)));
        }
    }
}