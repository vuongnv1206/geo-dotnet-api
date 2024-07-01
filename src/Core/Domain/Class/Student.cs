using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FSH.WebApi.Domain.Class;
public class Student : AuditableEntity, IAggregateRoot
{
    public Guid? StId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? AvatarUrl { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? StudentCode { get; set; }
    public bool? Gender { get; set; }

    public Student Update(
        string? firstName,
        string? lastName,
        string? avatarUrl,
        DateTime? dateOfBirth,
        string? email,
        string? phoneNumber,
        string? studentCode,
        bool? gender)
    {
        if (firstName is not null && !FirstName.Equals(firstName)) FirstName = firstName;
        if (lastName is not null && !LastName.Equals(lastName)) LastName = lastName;
        if (avatarUrl is not null && !AvatarUrl.Equals(avatarUrl)) AvatarUrl = avatarUrl;
        if (DateOfBirth != DateTime.MinValue && !DateOfBirth.Equals(dateOfBirth)) DateOfBirth = dateOfBirth;
        if (email is not null && !Email.Equals(email)) Email = email;
        if (phoneNumber is not null && !PhoneNumber.Equals(phoneNumber)) PhoneNumber = phoneNumber;
        if (studentCode is not null && !StudentCode.Equals(studentCode)) StudentCode = studentCode;
        if (gender.HasValue && Gender != gender.Value) Gender = gender.Value;

        return this;
    }

    public bool IsValidEmail(string? email)
    {
        var emailRegex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
        return emailRegex.IsMatch(email ?? string.Empty);
    }

    public bool IsValidPhoneNumber(string? phoneNumber)
    {
        var phoneRegex = new Regex(@"^\d{10}$");
        return phoneRegex.IsMatch(phoneNumber ?? string.Empty);
    }
}
