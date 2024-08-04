namespace FSH.WebApi.Infrastructure.Persistence.Configuration;

internal static class SchemaNames
{
    // TODO: figure out how to capitalize these only for Oracle
    public static string Auditing = nameof(Auditing); // "AUDITING";
    public static string Catalog = nameof(Catalog); // "CATALOG";
    public static string Assignment = nameof(Assignment); // "ASSIGNMENT";
    public static string Subject = nameof(Subject); // "SUBJECT";
    public static string Identity = nameof(Identity); // "IDENTITY";
    public static string MultiTenancy = nameof(MultiTenancy); // "MULTITENANCY";
    public static string Classroom = nameof(Classroom); // "CLASSROOM";
    public static string Question = nameof(Question); // "QUESTION";
    public static string GroupTeacher = nameof(GroupTeacher); // "GROUPTEACHER";
    public static string Examination = nameof(Examination); // "EXAMINATION";
    public static string Notification = nameof(Notification); // "NOTIFICATION";
    public static string Payment = nameof(Payment); // "PAYMENT";
}