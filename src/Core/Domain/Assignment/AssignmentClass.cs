using FSH.WebApi.Domain.Class;

namespace FSH.WebApi.Domain.Assignment;

public class AssignmentClass
{
    public DefaultIdType AssignmentId { get; set; }
    public DefaultIdType ClassesId { get; set; }
    public virtual Classes Classes { get; set; }
    public virtual Assignment Assignment { get; set; }

    public AssignmentClass()
    {
    }

    public AssignmentClass(DefaultIdType assignmentId, DefaultIdType classesId)
    {
        AssignmentId = assignmentId;
        ClassesId = classesId;
    }
}
