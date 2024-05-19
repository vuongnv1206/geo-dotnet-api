using FSH.WebApi.Domain.TeacherGroup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.TeacherGroup.TeacherInGroups;
public interface ITeacherInGroupRepository : IScopedService
{
    Task DeleteTeacherInGroupAsync(TeacherInGroup request);
    Task<TeacherInGroup> GetTeacherInGroup(TeacherInGroup request);
}
