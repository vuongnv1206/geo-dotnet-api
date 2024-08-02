using System.Collections.ObjectModel;

namespace FSH.WebApi.Shared.Authorization;

public static class FSHAction
{
    public const string View = nameof(View);
    public const string Search = nameof(Search);
    public const string Create = nameof(Create);
    public const string Update = nameof(Update);
    public const string Delete = nameof(Delete);
    public const string Export = nameof(Export);
    public const string Generate = nameof(Generate);
    public const string Clean = nameof(Clean);
    public const string UpgradeSubscription = nameof(UpgradeSubscription);
    public const string Upload = nameof(Upload);
}

public static class FSHResource
{
    public const string Tenants = nameof(Tenants);
    public const string Dashboard = nameof(Dashboard);
    public const string Hangfire = nameof(Hangfire);
    public const string Users = nameof(Users);
    public const string UserRoles = nameof(UserRoles);
    public const string Roles = nameof(Roles);
    public const string RoleClaims = nameof(RoleClaims);
    public const string Products = nameof(Products);
    public const string Brands = nameof(Brands);
    public const string QuestionFolders = nameof(QuestionFolders);
    public const string Question = nameof(Question);
    public const string QuestionLabel = nameof(QuestionLabel);
    public const string GroupTeachers = nameof(GroupTeachers);
    public const string Assignments = nameof(Assignments);
    public const string Subjects = nameof(Subjects);
    public const string Classes = nameof(Classes);
    public const string GroupClasses = nameof(GroupClasses);
    public const string News = nameof(News);
    public const string NewsReaction = nameof(NewsReaction);
    public const string UserClasses = nameof(UserClasses);
    public const string PaperFolders = nameof(PaperFolders);
    public const string Papers = nameof(Papers);
    public const string PaperLabels = nameof(PaperLabels);
    public const string TeacherTeams = nameof(TeacherTeams);
    public const string Files = nameof(Files);
    public const string Notifications = nameof(Notifications);
    public const string AuditLogs = nameof(AuditLogs);
}

public static class FSHPermissions
{
    private static readonly FSHPermission[] _all = new FSHPermission[]
    {
        new("View Dashboard", FSHAction.View, FSHResource.Dashboard),
        new("View Hangfire", FSHAction.View, FSHResource.Hangfire),

        // USERS
        new("View Users", FSHAction.View, FSHResource.Users),
        new("Search Users", FSHAction.Search, FSHResource.Users),
        new("Create Users", FSHAction.Create, FSHResource.Users),
        new("Update Users", FSHAction.Update, FSHResource.Users),
        new("Delete Users", FSHAction.Delete, FSHResource.Users),
        new("Export Users", FSHAction.Export, FSHResource.Users),

        // ROLES
        new("View UserRoles", FSHAction.View, FSHResource.UserRoles),
        new("Update UserRoles", FSHAction.Update, FSHResource.UserRoles),
        new("View Roles", FSHAction.View, FSHResource.Roles),
        new("Create Roles", FSHAction.Create, FSHResource.Roles),
        new("Update Roles", FSHAction.Update, FSHResource.Roles),
        new("Delete Roles", FSHAction.Delete, FSHResource.Roles),
        new("View RoleClaims", FSHAction.View, FSHResource.RoleClaims),
        new("Update RoleClaims", FSHAction.Update, FSHResource.RoleClaims),

        // TEACHERS
        new("View GroupTeachers", FSHAction.View, FSHResource.GroupTeachers),
        new("Search GroupTeachers", FSHAction.Search, FSHResource.GroupTeachers),
        new("Create GroupTeachers", FSHAction.Create, FSHResource.GroupTeachers),
        new("Update GroupTeachers", FSHAction.Update, FSHResource.GroupTeachers),
        new("Delete GroupTeachers", FSHAction.Delete, FSHResource.GroupTeachers),

        new("View Tenants", FSHAction.View, FSHResource.Tenants, IsRoot: true),
        new("Create Tenants", FSHAction.Create, FSHResource.Tenants, IsRoot: true),
        new("Update Tenants", FSHAction.Update, FSHResource.Tenants, IsRoot: true),
        new("Upgrade Tenant Subscription", FSHAction.UpgradeSubscription, FSHResource.Tenants, IsRoot: true),
        new("View QuestionFolders", FSHAction.View, FSHResource.QuestionFolders),

        // ASSIGNMENTS
        new("View Assignments", FSHAction.View, FSHResource.Assignments, IsStudent: true),
        new("Search Assignments", FSHAction.Search, FSHResource.Assignments, IsStudent: true),
        new("Create Assignments", FSHAction.Create, FSHResource.Assignments),
        new("Update Assignments", FSHAction.Update, FSHResource.Assignments),
        new("Delete Assignments", FSHAction.Delete, FSHResource.Assignments),
        new("Export Assignments", FSHAction.Export, FSHResource.Assignments),

        // SUBJECTS
        new("View Subjects", FSHAction.View, FSHResource.Subjects),
        new("Search Subjects", FSHAction.Search, FSHResource.Subjects),
        new("Create Subjects", FSHAction.Create, FSHResource.Subjects),
        new("Update Subjects", FSHAction.Update, FSHResource.Subjects),
        new("Delete Subjects", FSHAction.Delete, FSHResource.Subjects),

        // CLASSES
        new("View Classes", FSHAction.View, FSHResource.Classes, IsStudent: true),
        new("Search Classes", FSHAction.Search, FSHResource.Classes, IsStudent: true),
        new("Create Classes", FSHAction.Create, FSHResource.Classes),
        new("Update Classes", FSHAction.Update, FSHResource.Classes),
        new("Delete Classes", FSHAction.Delete, FSHResource.Classes),
        new("View GroupClasses", FSHAction.View, FSHResource.GroupClasses, IsStudent: true),
        new("Search GroupClasses", FSHAction.Search, FSHResource.GroupClasses, IsStudent: true),
        new("Create GroupClasses", FSHAction.Create, FSHResource.GroupClasses),
        new("Update GroupClasses", FSHAction.Update, FSHResource.GroupClasses),
        new("Delete GroupClasses", FSHAction.Delete, FSHResource.GroupClasses),

        // QUESTIONS FOLDERS
        new("View QuestionFolders", FSHAction.View, FSHResource.QuestionFolders),
        new("View News", FSHAction.View, FSHResource.News),
        new("Search News", FSHAction.Search, FSHResource.News),
        new("Create News", FSHAction.Create, FSHResource.News),
        new("Update News", FSHAction.Update, FSHResource.News),
        new("Delete News", FSHAction.Delete, FSHResource.News),


        // QUESTION LABELS
        new("View QuestionLabels", FSHAction.View, FSHResource.QuestionLabel, IsStudent: true),
        new("Search QuestionLabels", FSHAction.Search, FSHResource.QuestionLabel, IsStudent: true),
        new("Create QuestionLabels", FSHAction.Create, FSHResource.QuestionLabel),
        new("Update QuestionLabels", FSHAction.Update, FSHResource.QuestionLabel),
        new("Delete QuestionLabels", FSHAction.Delete, FSHResource.QuestionLabel),

        //NEWS REACTIONS
        new("View NewsReaction", FSHAction.View, FSHResource.NewsReaction, IsStudent: true),
        new("Search NewsReaction", FSHAction.Search, FSHResource.NewsReaction, IsStudent: true),
        new("Create NewsReaction", FSHAction.Create, FSHResource.NewsReaction),
        new("Update NewsReaction", FSHAction.Update, FSHResource.NewsReaction),
        new("Delete NewsReaction", FSHAction.Delete, FSHResource.NewsReaction),
        new("Create QuestionFolders", FSHAction.Create, FSHResource.QuestionFolders),
        new("Update QuestionFolders", FSHAction.Update, FSHResource.QuestionFolders),
        new("Delete QuestionFolders", FSHAction.Delete, FSHResource.QuestionFolders),

        // QUESTIONS
        new("View Question", FSHAction.View, FSHResource.Question),
        new("Create Question", FSHAction.Create, FSHResource.Question),
        new("Update Question", FSHAction.Update, FSHResource.Question),
        new("Delete Question", FSHAction.Delete, FSHResource.Question),

        // USER CLASSES
        new("View UserClasses", FSHAction.View, FSHResource.UserClasses, IsStudent: true),
        new("Search UserClasses", FSHAction.Search, FSHResource.UserClasses),
        new("Create UserClasses", FSHAction.Create, FSHResource.UserClasses),
        new("Update UserClasses", FSHAction.Update, FSHResource.UserClasses),
        new("Delete UserClasses", FSHAction.Delete, FSHResource.UserClasses),

        // PAPER FOLDERS
        new("View PaperFolders", FSHAction.View, FSHResource.PaperFolders),
        new("Create PaperFolders", FSHAction.Create, FSHResource.PaperFolders),
        new("Update PaperFolders", FSHAction.Update, FSHResource.PaperFolders),
        new("Delete PaperFolders", FSHAction.Delete, FSHResource.PaperFolders),

        // PAPERS
        new("View Papers", FSHAction.View, FSHResource.Papers),
        new("Create Papers", FSHAction.Create, FSHResource.Papers),
        new("Update Papers", FSHAction.Update, FSHResource.Papers),
        new("Delete Papers", FSHAction.Delete, FSHResource.Papers),

        //PAPER LABELS
        new("View PaperLabels", FSHAction.View, FSHResource.PaperLabels),
        new("Create PaperLabels", FSHAction.Create, FSHResource.PaperLabels),
        new("Update PaperLabels", FSHAction.Update, FSHResource.PaperLabels),
        new("Delete PaperLabels", FSHAction.Delete, FSHResource.PaperLabels),

        // TEACHER TEAMS
        new("View TeacherTeams", FSHAction.View, FSHResource.TeacherTeams),
        new("Create TeacherTeams", FSHAction.Create, FSHResource.TeacherTeams),
        new("Update TeacherTeams", FSHAction.Update, FSHResource.TeacherTeams),
        new("Delete TeacherTeams", FSHAction.Delete, FSHResource.TeacherTeams),

        //GROUP TEACHERS
        new("View GroupTeachers", FSHAction.View, FSHResource.GroupTeachers),
        new("Create GroupTeachers", FSHAction.Create, FSHResource.GroupTeachers),
        new("Update GroupTeachers", FSHAction.Update, FSHResource.GroupTeachers),
        new("Delete GroupTeachers", FSHAction.Delete, FSHResource.GroupTeachers),

        // FILES
        new("Upload files", FSHAction.Upload, FSHResource.Files, IsRoot: true),

        // NOTIFICATIONS
        new("Send Notifications", FSHAction.Create, FSHResource.Notifications, IsRoot: true),

        // AUDIT LOGS
        new("View AuditLogs", FSHAction.View, FSHResource.AuditLogs),

    };

    public static IReadOnlyList<FSHPermission> All { get; } = new ReadOnlyCollection<FSHPermission>(_all);
    public static IReadOnlyList<FSHPermission> Root { get; } = new ReadOnlyCollection<FSHPermission>(_all.Where(p => p.IsRoot).ToArray());
    public static IReadOnlyList<FSHPermission> Teacher { get; } = new ReadOnlyCollection<FSHPermission>(_all.Where(p => !p.IsRoot).ToArray());
    public static IReadOnlyList<FSHPermission> Student { get; } = new ReadOnlyCollection<FSHPermission>(_all.Where(p => p.IsStudent).ToArray());

}

public record FSHPermission(string Description, string Action, string Resource, bool IsStudent = false, bool IsRoot = false)
{
    public string Name => NameFor(Action, Resource);
    public static string NameFor(string action, string resource) => $"Permissions.{resource}.{action}";
}
