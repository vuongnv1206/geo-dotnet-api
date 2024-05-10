

namespace FSH.WebApi.Domain.Examination;
public class PaperLable : AuditableEntity, IAggregateRoot
{
    public string Name { get; set; } = null!;
    public PaperLable Update(string? name)
    {
        if (name is not null && Name?.Equals(name) is not true) Name = name;
        return this;
    }
}

