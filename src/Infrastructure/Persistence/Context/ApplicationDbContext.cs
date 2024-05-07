using Finbuckle.MultiTenant;
using FSH.WebApi.Application.Common.Events;
using FSH.WebApi.Application.Common.Interfaces;
using FSH.WebApi.Domain.Catalog;
using FSH.WebApi.Domain.Question;
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

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Brand> Brands => Set<Brand>();
    public DbSet<QuestionFolder> QuestionFolders => Set<QuestionFolder>();
    public DbSet<QuestionFolderPermission> QuestionFolderPermissions => Set<QuestionFolderPermission>();
    public DbSet<TeacherInGroup> TeacherInGroups { get; set; }
    public DbSet<GroupTeacher> GroupTeachers { get; set; }
    public DbSet<TeacherPermissionInClass> TeacherPermissionInClasses { get; set; }
    public DbSet<TeacherTeam> TeacherTeams { get; set; }
    public DbSet<GroupPermissionInClass> GroupPermissionInClasses { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema(SchemaNames.Catalog);

    }
}