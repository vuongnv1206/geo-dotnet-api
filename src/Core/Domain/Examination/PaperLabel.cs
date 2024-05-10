

namespace FSH.WebApi.Domain.Examination;
public class PaperLabel : AuditableEntity, IAggregateRoot
{
    public string Name { get; set; } = null!;
    public PaperLabel Update(string? name)
    {
        if (name is not null && Name?.Equals(name) is not true) Name = name;
        return this;
    }
    public PaperLabel(string name)
    {
        Name = name;
    }
}

