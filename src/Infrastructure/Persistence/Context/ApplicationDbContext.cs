using Finbuckle.MultiTenant;
using FSH.WebApi.Application.Common.Events;
using FSH.WebApi.Application.Common.Interfaces;
using FSH.WebApi.Domain.Assignment;
using FSH.WebApi.Domain.Class;
using FSH.WebApi.Domain.Examination;
using FSH.WebApi.Domain.Notification;
using FSH.WebApi.Domain.Payment;
using FSH.WebApi.Domain.Question;
using FSH.WebApi.Domain.Subjects;
using FSH.WebApi.Domain.TeacherGroup;
using FSH.WebApi.Infrastructure.Persistence.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FSH.WebApi.Infrastructure.Persistence.Context;

public class ApplicationDbContext : BaseDbContext
{
    public ApplicationDbContext(ITenantInfo currentTenant, DbContextOptions options, ICurrentUser currentUser, ISerializerService serializer, IOptions<DatabaseSettings> dbSettings, IEventPublisher events)
        : base(currentTenant, options, currentUser, serializer, dbSettings, events)
    {
    }

    public DbSet<Assignment> Assignments => Set<Assignment>();
    public DbSet<AssignmentStudent> AssignmentStudent => Set<AssignmentStudent>();
    public DbSet<AssignmentClass> AssignmentClass => Set<AssignmentClass>();
    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<Classes> Classes => Set<Classes>();
    public DbSet<GroupClass> GroupClasses => Set<GroupClass>();
    public DbSet<Post> Post => Set<Post>();
    public DbSet<UserClass> UserClasses => Set<UserClass>();
    public DbSet<PostLike> PostLike => Set<PostLike>();
    public DbSet<QuestionFolder> QuestionFolders => Set<QuestionFolder>();
    public DbSet<Domain.Question.Question> Questions => Set<Domain.Question.Question>();
    public DbSet<QuestionLable> QuestionLables => Set<QuestionLable>();
    public DbSet<Answer> Answers => Set<Answer>();
    public DbSet<QuestionFolderPermission> QuestionFolderPermissions => Set<QuestionFolderPermission>();
    public DbSet<TeacherInGroup> TeacherInGroups { get; set; }
    public DbSet<GroupTeacher> GroupTeachers { get; set; }
    public DbSet<TeacherPermissionInClass> TeacherPermissionInClasses { get; set; }
    public DbSet<TeacherTeam> TeacherTeams { get; set; }
    public DbSet<GroupPermissionInClass> GroupPermissionInClasses { get; set; }
    public DbSet<Paper> Papers { get; set; }
    public DbSet<PaperFolder> PaperFolders { get; set; }
    public DbSet<PaperFolderPermission> PaperFolderPermissions { get; set; }
    public DbSet<PaperLabel> PaperLabels { get; set; }
    public DbSet<PaperQuestion> PaperQuestions { get; set; }
    public DbSet<SubmitPaper> SubmitPapers { get; set; }
    public DbSet<SubmitPaperDetail> SubmitPaperDetails { get; set; }
    public DbSet<SubmitPaperLog> SubmitPaperLogs { get; set; }
    public DbSet<PaperAccess> PaperAccesses { get; set; }
    public DbSet<PaperPermission> PaperPermissions { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<QuestionClone> QuestionClones { get; set; }
    public DbSet<AnswerClone> AnswerClones { get; set; }
    public DbSet<PaperMatrix> PaperMatrices { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<JoinGroupTeacherRequest> JoinGroupTeacherRequests { get; set; }
    public DbSet<JoinTeacherTeamRequest> JoinTeacherTeamRequests { get; set; }
    public DbSet<InviteJoinTeacherTeam> InviteJoinTeacherTeams { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        _ = modelBuilder.HasDefaultSchema(SchemaNames.Catalog);
    }
}