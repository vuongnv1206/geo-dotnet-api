using FSH.WebApi.Domain.Class;

namespace FSH.WebApi.Application.Class.UserClasses;
public interface IUserClassesRepository : IScopedService
{
    Task AddNewUserInClass(UserClass request);

    Task DeleteUserInClass(Guid userId, Guid classId);

    Task<List<UserClass>> GetUserInClasses(Guid classId);

    Task<UserClass> GetUserDetailInClasses(Guid userId, Guid classesId);

    Task<int> GetNumberUserOfClasses(Guid classesId);
}
