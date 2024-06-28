using FSH.WebApi.Domain.Class;

namespace FSH.WebApi.Application.Auditing.Class;
public class ClassLogDto : ICloneable
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

    public object Clone()
    {
        return this.MemberwiseClone();
    }

    public void SetClassChangeField(string field, string value)
    {
        if (string.IsNullOrEmpty(field) || string.IsNullOrEmpty(value)) return;
        switch (field)
        {
            case "Name":
                Name = value;
                break;
            case "SchoolYear":
                SchoolYear = value;
                break;
            case "GroupClassId":
                GroupClassId = Guid.Parse(value);
                break;
        }
    }
}
