using FSH.WebApi.Domain.Class;


namespace FSH.WebApi.Application.Class.GroupClasses;
public class GroupClassByIdSpec : Specification<GroupClass>
{
    public GroupClassByIdSpec(Guid id)
    {
        Query.Include(x => x.Classes).Where(x => x.Id == id);
    }
}
