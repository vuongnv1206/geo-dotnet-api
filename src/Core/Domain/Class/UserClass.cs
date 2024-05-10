using FSH.WebApi.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Domain.Class;
public class UserClass : AuditableEntity, IAggregateRoot
{
    public Guid ClassId { get; private set; }
    public Guid UserId { get; private set; }
    public bool IsGender { get; private set; }
    public string StudentCode { get; private set; }
    public string Email { get; private set; }
    public string PhoneNumber { get; private set; }
    public virtual Classes Classes { get; private set; }

    public UserClass(Guid classId, Guid userId, bool isGender, string studentCode, string email, string phoneNumber)
    {
        ClassId = classId;
        UserId = userId;
        IsGender = isGender;
        StudentCode = studentCode;
        Email = email;
        PhoneNumber = phoneNumber;
    }


}
