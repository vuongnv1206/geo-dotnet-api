﻿using Finbuckle.MultiTenant.EntityFrameworkCore;
using FSH.WebApi.Domain.Catalog;
using FSH.WebApi.Domain.Question;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSH.WebApi.Infrastructure.Persistence.Configuration;
public class QuestionFolderConfig : IEntityTypeConfiguration<QuestionFolder>
{
    public void Configure(EntityTypeBuilder<QuestionFolder> builder)
    {
        builder
            .ToTable("QuestionFolders", SchemaNames.Question)
            .IsMultiTenant();

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasOne(x => x.Parent)
            .WithMany(x => x.Children)
            .HasForeignKey(x => x.ParentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class QuestionFolderPermissionConfig : IEntityTypeConfiguration<QuestionFolderPermission>
{
    public void Configure(EntityTypeBuilder<QuestionFolderPermission> builder)
    {
        builder
            .ToTable("QuestionFolderPermissions", SchemaNames.Question)
            .IsMultiTenant();

        builder.HasKey(x => new { x.QuestionFolderId, x.UserId });

        builder.HasOne(x => x.QuestionFolder)
            .WithMany(x => x.Permissions)
            .HasForeignKey(x => x.QuestionFolderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
