using System.Collections.ObjectModel;

namespace FSH.WebApi.Shared.Authorization;

public static class FSHRoles
{
    public const string Teacher = nameof(Teacher);
    public const string Student = nameof(Student);

    public static IReadOnlyList<string> DefaultRoles { get; } = new ReadOnlyCollection<string>(new[]
    {
        Teacher,
        Student
    });

    public static bool IsDefault(string roleName) => DefaultRoles.Any(r => r == roleName);
}