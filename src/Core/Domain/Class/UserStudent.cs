using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FSH.WebApi.Domain.Class;
public class UserStudent : AuditableEntity, IAggregateRoot
{
    public Guid? StudentId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? StudentCode { get; set; }
    public bool? Gender { get; set; }


    public UserStudent Update(string? firstName, string? lastName, string? studentEmail, string? studentEPhoneNumber, string? studentCode, bool? gender)
    {
        if (firstName is not null && FirstName?.Equals(firstName) is not true) FirstName = firstName;
        if (lastName is not null && LastName?.Equals(LastName) is not true) LastName = lastName;
        if (studentEmail is not null && Email?.Equals(studentEmail) is not true) Email = studentEmail;
        if (studentEPhoneNumber is not null && PhoneNumber?.Equals(studentEPhoneNumber) is not true) PhoneNumber = studentEPhoneNumber;
        if (gender.HasValue && Gender != gender.Value) Gender = gender.Value;
        return this;
    }

    public bool IsValidEmail(string? email)
    {
        var emailRegex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
        return emailRegex.IsMatch(email);
    }

    public bool IsValidPhoneNumber(string? phoneNumber)
    {
        var phoneRegex = new Regex(@"^\d{10}$");
        return phoneRegex.IsMatch(phoneNumber);
    }
}
