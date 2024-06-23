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
    public const string GroupTeachers = nameof(GroupTeachers);
    public const string Assignments = nameof(Assignments);
    public const string Subjects = nameof(Subjects);
    public const string Classes = nameof(Classes);
    public const string GroupClasses = nameof(GroupClasses);
    public const string News = nameof(News);
    public const string NewsReaction = nameof(NewsReaction);
    public const string UserClasses = nameof(UserClasses);
    public const string Files = nameof(Files);
    public const string Notifications = nameof(Notifications);
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

        new("View Products", FSHAction.View, FSHResource.Products, IsStudent: true),
        new("Search Products", FSHAction.Search, FSHResource.Products, IsStudent: true),
        new("Create Products", FSHAction.Create, FSHResource.Products),
        new("Update Products", FSHAction.Update, FSHResource.Products),
        new("Delete Products", FSHAction.Delete, FSHResource.Products),
        new("Export Products", FSHAction.Export, FSHResource.Products),
        new("View Brands", FSHAction.View, FSHResource.Brands, IsStudent: true),
        new("Search Brands", FSHAction.Search, FSHResource.Brands, IsStudent: true),
        new("Create Brands", FSHAction.Create, FSHResource.Brands),
        new("Update Brands", FSHAction.Update, FSHResource.Brands),
        new("Delete Brands", FSHAction.Delete, FSHResource.Brands),
        new("Generate Brands", FSHAction.Generate, FSHResource.Brands),
        new("Clean Brands", FSHAction.Clean, FSHResource.Brands),

        // TEACHERS
        new("View GroupTeachers", FSHAction.View, FSHResource.GroupTeachers, IsStudent: true),
        new("Search GroupTeachers", FSHAction.Search, FSHResource.GroupTeachers, IsStudent: true),
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
        new("View Subjects", FSHAction.View, FSHResource.Subjects, IsStudent: true),
        new("Search Subjects", FSHAction.Search, FSHResource.Subjects, IsStudent: true),
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
        new("Search UserClasses", FSHAction.Search, FSHResource.UserClasses, IsStudent: true),
        new("Create UserClasses", FSHAction.Create, FSHResource.UserClasses),
        new("Update UserClasses", FSHAction.Update, FSHResource.UserClasses),
        new("Delete UserClasses", FSHAction.Delete, FSHResource.UserClasses),

        // FILES
        new("Upload files", FSHAction.Upload, FSHResource.Files, IsRoot: true),

        // NOTIFICATIONS
        new("Send Notifications", FSHAction.Create, FSHResource.Notifications, IsRoot: true),

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
