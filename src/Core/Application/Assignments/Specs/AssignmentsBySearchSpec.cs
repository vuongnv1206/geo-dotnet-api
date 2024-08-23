using FSH.WebApi.Application.Assignments.Dtos;
using FSH.WebApi.Domain.Assignment;
using FSH.WebApi.Domain.TeacherGroup;

namespace FSH.WebApi.Application.Assignments;
public class AssignmentsBySearchSpec : EntitiesByPaginationFilterSpec<Assignment, AssignmentDto>
{
    public AssignmentsBySearchSpec(SearchMyAssignmentsRequest request, Guid userId, bool isInRoleStudent)
        : base(request)
    {
        if (isInRoleStudent)  //Lấy những assignment đc giao
        {
            Query
                .Include(p => p.Subject)
                .Include(a => a.AssignmentClasses)
                    .ThenInclude(ac => ac.Classes)
                .Include(a => a.AssignmentStudents)
                    .ThenInclude(assignStudent => assignStudent.Student)
                    .Where(p => p.SubjectId.Equals(request.SubjectId!.Value), request.SubjectId.HasValue)     //Filter by Subject
                    .Where(a => a.AssignmentClasses.Any(ac => ac.ClassesId == request.ClassId), request.ClassId.HasValue)   //Filter by Class
                    .Where(a => a.AssignmentStudents.Any(assignStudent => assignStudent.Student.StId == userId))
                    .OrderBy(a => a.Name, !request.HasOrderBy())
                ;
        }
        else  //Teacher or Manager
        {
            Query
                .Include(p => p.Subject)
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
                .Where(p => p.SubjectId.Equals(request.SubjectId!.Value), request.SubjectId.HasValue)
               
                .Where(a => a.CreatedBy == userId
              || a.AssignmentClasses.Any(ac => ac.Classes.TeacherPermissionInClasses.Any(tp => tp.TeacherTeam.TeacherId == userId && (tp.PermissionType == PermissionType.AssignAssignment || tp.PermissionType == PermissionType.Marking)))
              || a.AssignmentClasses.Any(ac => ac.Classes.GroupPermissionInClasses.Any(gp => (gp.PermissionType == PermissionType.AssignAssignment || gp.PermissionType == PermissionType.Marking) && gp.GroupTeacher.TeacherInGroups
                              .Any(tig => tig.TeacherTeam.TeacherId == userId))))

                 .OrderBy(c => c.Name, !request.HasOrderBy());
        }

    }
}

