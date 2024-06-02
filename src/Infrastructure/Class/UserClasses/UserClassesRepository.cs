using FSH.WebApi.Application.Class.UserClasses;
using FSH.WebApi.Domain.Class;
using FSH.WebApi.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace FSH.WebApi.Infrastructure.Class.UserClasses;
public class UserClassesRepository : IUserClassesRepository
{
    private readonly ApplicationDbContext _context;

    public UserClassesRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddNewUserInClass(UserClass request)
    {
        _context.UserClasses.Add(request);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteUserInClass(Guid userId, Guid classId)
    {
        var userClasses = await _context.UserClasses.FirstOrDefaultAsync(c => c.UserId == userId && c.ClassesId == classId);
        if (userClasses != null)
        {
            _context.UserClasses.Remove(userClasses);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<UserClass> GetUserDetailInClasses(Guid userId, Guid classesId)
    {
        return await _context.UserClasses.FirstOrDefaultAsync(t => t.UserId == userId && t.ClassesId == classesId);
    }

    public async Task<List<UserClass>> GetUserInClasses(Guid classesId)
    {
        return await _context.UserClasses
         .Where(uc => uc.ClassesId == classesId)
         .ToListAsync();
    }

    public async Task UpdateUserInClasses(UserClass request)
    {
        _context.UserClasses.Update(request);
        await _context.SaveChangesAsync();

    }
}
