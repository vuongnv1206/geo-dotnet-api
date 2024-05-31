using FSH.WebApi.Domain.Class;

namespace FSH.WebApi.Domain.Assignment;

public class AssignmentClass
{
    public Guid AssignmentId { get; private set; }
    public Guid ClassesId { get; private set; }
    public virtual Classes Classes { get; private set; }
    public virtual Assignment Assignment { get; private set; }
    public Guid ClassId { get; private set; }

    // public virtual Class Class { get; private set; } = default!;

    public AssignmentClass()
    {

    }

    public AssignmentClass(DefaultIdType assignmentId, DefaultIdType classesId)
    {
        AssignmentId = assignmentId;
        ClassesId = classesId;
    }
}
