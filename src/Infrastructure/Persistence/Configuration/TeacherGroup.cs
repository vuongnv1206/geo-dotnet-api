using Finbuckle.MultiTenant.EntityFrameworkCore;
using FSH.WebApi.Domain.TeacherGroup;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace FSH.WebApi.Infrastructure.Persistence.Configuration;
public class TeacherPermissionConfig : IEntityTypeConfiguration<TeacherPermission>
{
    public void Configure(EntityTypeBuilder<TeacherPermission> builder)
    {
        builder.IsMultiTenant();
        builder.ToTable("TeacherPermissions", SchemaNames.GroupTeacher);
        builder.Property(b => b.CanAssignExamination).HasDefaultValue(false);
        builder.Property(b => b.CanMarking).HasDefaultValue(false);
        builder.Property(b => b.CanManagementStudent).HasDefaultValue(false);
    }
}

public class GroupTeacherConfig : IEntityTypeConfiguration<GroupTeacher>
{
    public void Configure(EntityTypeBuilder<GroupTeacher> builder)
    {
        builder.IsMultiTenant();
        builder.ToTable("GroupTeachers", SchemaNames.GroupTeacher);
        builder
            .Property(b => b.Name)
                .HasMaxLength(256);
    }
}

public class TeacherInGroupConfig : IEntityTypeConfiguration<TeacherInGroup>
{
    public void Configure(EntityTypeBuilder<TeacherInGroup> builder)
    {
        builder.IsMultiTenant();
        builder.HasKey(tig => new { tig.TeacherId, tig.GroupTeacherId });
        builder.ToTable("TeacherInGroups", SchemaNames.GroupTeacher);
    }
}

public class TeacherTeamConfig : IEntityTypeConfiguration<TeacherTeam>
{
    public void Configure(EntityTypeBuilder<TeacherTeam> builder)
    {
        builder.IsMultiTenant();
        builder.ToTable("TeacherTeams", SchemaNames.GroupTeacher);
        builder.Property(b => b.TeacherName).HasMaxLength(50);
        builder.Property(b => b.Phone).HasMaxLength(20);
        builder.Property(b => b.Email).HasMaxLength(20);
    }
}
