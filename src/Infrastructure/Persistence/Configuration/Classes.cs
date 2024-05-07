using Finbuckle.MultiTenant.EntityFrameworkCore;
using FSH.WebApi.Domain.Catalog;
using FSH.WebApi.Domain.Class;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Infrastructure.Persistence.Configuration;

public class ClassesConfig : IEntityTypeConfiguration<Classes>
{
    public void Configure(EntityTypeBuilder<Classes> builder)
    {
        builder.ToTable("Classes", SchemaNames.Classes).IsMultiTenant();

        builder.Property(b => b.Name).HasMaxLength(256);
        builder.Property(b => b.SchoolYear).HasMaxLength(256);
        builder.Property(b => b.OwnerId).HasMaxLength(256);
        builder.Property(b => b.GroupClassId).HasMaxLength(256);
    }
}

public class GroupClassConfig : IEntityTypeConfiguration<GroupClass>
{
    public void Configure(EntityTypeBuilder<GroupClass> builder)
    {
        builder.Property(b => b.Name).HasMaxLength(256);

        builder.ToTable("GroupClasses", SchemaNames.Classes).IsMultiTenant();
    }
}

public class NewsConfig : IEntityTypeConfiguration<News>
{
    public void Configure(EntityTypeBuilder<News> builder)
    {
        builder.ToTable("News", SchemaNames.Classes).IsMultiTenant();

        builder.Property(b => b.ClassId).HasMaxLength(256);
        builder.Property(b => b.Content).HasMaxLength(2048);
        builder.Property(b => b.IsLockCommnet).HasMaxLength(20);
        builder.Property(b => b.ParentId).HasMaxLength(256);
    }
}

public class NewsReactionConfig : IEntityTypeConfiguration<NewsReaction>
{
    public void Configure(EntityTypeBuilder<NewsReaction> builder)
    {

        builder.IsMultiTenant();
        builder.HasKey(b => new { b.UserId, b.NewsId });
        builder.ToTable("NewsReactions", SchemaNames.Classes);
    }
}

public class UserClassConfig : IEntityTypeConfiguration<UserClass>
{
    public void Configure(EntityTypeBuilder<UserClass> builder)
    {
        builder.IsMultiTenant();
        builder.HasKey(b => new { b.UserId, b.ClassId });
        builder.Property(b => b.Email).HasMaxLength(256);
        builder.Property(b => b.IsGender).HasMaxLength(20);
        builder.Property(b => b.StudentCode).HasMaxLength(256);
        builder.Property(b => b.PhoneNumber).HasMaxLength(256);
        builder.ToTable("UserClasses", SchemaNames.Classes);
    }
}
