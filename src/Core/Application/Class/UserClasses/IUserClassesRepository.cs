using FSH.WebApi.Domain.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class.UserClasses;
public interface IUserClassesRepository : IScopedService
{
    Task AddNewUserInClass(UserClass request);

    Task DeleteUserInClass(Guid userId, Guid classId);

    Task UpdateUserInClasses(UserClass request);

    Task<List<UserClass>> GetUserInClasses(Guid classId);

    Task<UserClass> GetUserDetailInClasses(Guid userId, Guid classesId);

    Task<int> GetNumberUserOfClasses(Guid classesId);
}
