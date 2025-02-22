﻿using Finbuckle.MultiTenant.EntityFrameworkCore;
using FSH.WebApi.Domain.TeacherGroup;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSH.WebApi.Infrastructure.Persistence.Configuration;
public class TeacherPermissionConfig : IEntityTypeConfiguration<TeacherPermissionInClass>
{
    public void Configure(EntityTypeBuilder<TeacherPermissionInClass> builder)
    {
        builder.IsMultiTenant();
        builder.ToTable("TeacherPermissions", SchemaNames.GroupTeacher);

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
        builder.HasKey(tig => new { tig.TeacherTeamId, tig.GroupTeacherId });
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
    }
}

public class TeacherPermissionInClassConfig : IEntityTypeConfiguration<TeacherPermissionInClass>
{
    public void Configure(EntityTypeBuilder<TeacherPermissionInClass> builder)
    {
        builder.IsMultiTenant();
        builder.ToTable("TeacherPermissionInClasses", SchemaNames.GroupTeacher);
    }
}

public class GroupPermissionInClassConfig : IEntityTypeConfiguration<GroupPermissionInClass>
{
    public void Configure(EntityTypeBuilder<GroupPermissionInClass> builder)
    {
        builder.IsMultiTenant();
        builder.ToTable("GroupPermissionInClasses", SchemaNames.GroupTeacher);
    }
}

public class JoinGroupTeacherRequestConfig : IEntityTypeConfiguration<JoinGroupTeacherRequest>
{
    public void Configure(EntityTypeBuilder<JoinGroupTeacherRequest> builder)
    {
        builder.IsMultiTenant();
        builder.ToTable("JoinGroupTeacherRequest", SchemaNames.GroupTeacher);
    }
}
public class JoinTeacherTeacherRequestConfig : IEntityTypeConfiguration<JoinTeacherTeamRequest>
{
    public void Configure(EntityTypeBuilder<JoinTeacherTeamRequest> builder)
    {
        builder.IsMultiTenant();
        builder.ToTable("JoinTeacherTeamRequest", SchemaNames.GroupTeacher);
    }
}

public class InviteJoinTeacherTeamConfig : IEntityTypeConfiguration<InviteJoinTeacherTeam>
{
    public void Configure(EntityTypeBuilder<InviteJoinTeacherTeam> builder)
    {
        builder.IsMultiTenant();
        builder.ToTable("InviteJoinTeacherTeam", SchemaNames.GroupTeacher);
    }
}

