using FSH.WebApi.Domain.Class;

namespace FSH.WebApi.Application.Auditing.Class;
public class ClassLogDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string SchoolYear { get; set; }
    public Guid? GroupClassId { get; set; }
    public string? GroupName { get; set; }

    public static ClassLogDto FromClass(Classes classes)
    {
        return new ClassLogDto
        {
            Id = classes.Id,
            Name = classes.Name,
            SchoolYear = classes.SchoolYear,
            GroupName = classes.GroupClass?.Name
        };
    }
}
