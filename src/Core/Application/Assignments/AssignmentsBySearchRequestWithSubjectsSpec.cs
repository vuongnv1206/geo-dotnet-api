using FSH.WebApi.Application.Assignments.Dtos;
using FSH.WebApi.Domain.Assignment;
using FSH.WebApi.Domain.TeacherGroup;

namespace FSH.WebApi.Application.Assignments;
public class AssignmentsBySearchRequestWithSubjectsSpec : EntitiesByPaginationFilterSpec<Assignment, AssignmentDto>
{
    public AssignmentsBySearchRequestWithSubjectsSpec(SearchAssignmentsRequest request, Guid userId)
        : base(request) =>
        Query
            .Include(p => p.Subject)
            .OrderBy(c => c.Name, !request.HasOrderBy())
            .Where(p => p.SubjectId.Equals(request.SubjectId!.Value), request.SubjectId.HasValue)
        .Include(a => a.AssignmentClasses)
            .ThenInclude(ac => ac.Classes)
                .ThenInclude(c => c.UserClasses)
                    .ThenInclude(uc => uc.Student)
        .Include(a => a.AssignmentClasses)
            .ThenInclude(ac => ac.Classes)
                .ThenInclude(c => c.GroupPermissionInClasses)
                        .ThenInclude(gp => gp.GroupTeacher)
                            .ThenInclude(gt => gt.TeacherInGroups)
                                .ThenInclude(tg => tg.TeacherTeam)
         .Include(a => a.AssignmentClasses)
            .ThenInclude(ac => ac.Classes)
                .ThenInclude(c => c.TeacherPermissionInClasses)
                    .ThenInclude(tp => tp.TeacherTeam)
        .Where(a => a.AssignmentClasses.Any(ac => ac.ClassesId == request.ClassId), request.ClassId.HasValue)
        .Include(a => a.AssignmentStudents)
            .ThenInclude(assignStudent => assignStudent.Student)
        .Where(a => a.CreatedBy == userId
                || a.AssignmentClasses.Any(ac => ac.Classes.UserClasses.Any(uc => uc.Student.StId == userId))
                || a.AssignmentStudents.Any(assignStudent => assignStudent.Student.StId == userId)
                || a.AssignmentClasses.Any(ac => ac.Classes.TeacherPermissionInClasses.Any(tp => tp.TeacherTeam.TeacherId == userId && (tp.PermissionType == PermissionType.AssignAssignment || tp.PermissionType == PermissionType.Marking)))
                || a.AssignmentClasses.Any(ac => ac.Classes.GroupPermissionInClasses.Any(gp => (gp.PermissionType == PermissionType.AssignAssignment || gp.PermissionType == PermissionType.Marking) && gp.GroupTeacher.TeacherInGroups
                                .Any(tig => tig.TeacherTeam.TeacherId == userId))));
}
