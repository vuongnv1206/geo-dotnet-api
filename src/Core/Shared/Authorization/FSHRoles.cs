using System.Collections.ObjectModel;

namespace FSH.WebApi.Shared.Authorization;

public static class FSHRoles
{
    public const string Teacher = nameof(Teacher);
    public const string Student = nameof(Student);
    public const string Basic = nameof(Basic);
    public const string Standard = nameof(Standard);
    public const string Professional = nameof(Professional);

    public static IReadOnlyList<string> DefaultRoles { get; } = new ReadOnlyCollection<string>(new[]
    {
        Teacher,
        Student,
        Basic,
        Standard,
        Professional
    });

    public static bool IsDefault(string roleName) => DefaultRoles.Any(r => r == roleName);
}