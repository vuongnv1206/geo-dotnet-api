using FSH.WebApi.Application.Common.Exceptions;
using FSH.WebApi.Application.TeacherGroup.TeacherInGroups;
using FSH.WebApi.Domain.TeacherGroup;
using FSH.WebApi.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Infrastructure.TeacherGroup.TeacherInGroups;
public class TeacherInGroupRepository : ITeacherInGroupRepository
{
    private readonly ApplicationDbContext _context;

    public TeacherInGroupRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddTeacherIntoGroup(TeacherInGroup request)
    {
        _context.TeacherInGroups.Add(request);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteTeacherInGroupAsync(TeacherInGroup request)
    {
        _context.TeacherInGroups.Remove(request);
        await _context.SaveChangesAsync();
    }

    public async Task<TeacherInGroup> GetTeacherInGroup(TeacherInGroup request) =>
       await _context.TeacherInGroups.FirstOrDefaultAsync(t => t.GroupTeacherId == request.GroupTeacherId
                                                    && t.TeacherTeamId == request.TeacherTeamId);
}
