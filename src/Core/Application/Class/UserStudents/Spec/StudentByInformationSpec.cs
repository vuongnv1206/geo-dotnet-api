using FSH.WebApi.Domain.Class;
using FSH.WebApi.Domain.TeacherGroup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class.UserStudents.Spec;
public class StudentByInformationSpec : Specification<Student>, ISingleResultSpecification

{
    public StudentByInformationSpec(string email, string phoneNumber, DefaultIdType userId) {
        Query.Where(x => (x.Email.Equals(email) || x.PhoneNumber.Equals(phoneNumber)) && x.CreatedBy == userId);
    }
}
