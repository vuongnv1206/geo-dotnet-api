using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Domain.Class;
public class GroupClass : AuditableEntity, IAggregateRoot
{
    public string Name { get; private set; }

    public GroupClass(string name)
    {
        Name = name;
    }

    public GroupClass Update(string? name)
    {
        if (name is not null && Name?.Equals(name) is not true) Name = name;
        return this;
    }
}
