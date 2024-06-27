using Finbuckle.MultiTenant.EntityFrameworkCore;
using FSH.WebApi.Domain.Class;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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

public class NewsConfig : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.ToTable("Post", SchemaNames.Classes).IsMultiTenant();

    }
}

public class NewsReactionConfig : IEntityTypeConfiguration<PostLike>
{
    public void Configure(EntityTypeBuilder<PostLike> builder)
    {
        builder.IsMultiTenant();
        builder.HasKey(b => new { b.UserId, b.PostId });
        builder.ToTable("NewsReactions", SchemaNames.Classes);
    }
}

public class UserClassConfig : IEntityTypeConfiguration<UserClass>
{
    public void Configure(EntityTypeBuilder<UserClass> builder)
    {
        builder.IsMultiTenant();
        builder.HasKey(b => new { b.UserStudentId, b.ClassesId });
        builder.ToTable("UserClasses", SchemaNames.Classes);
    }
}

public class UserStudentConfig : IEntityTypeConfiguration<UserStudent>
{
    public void Configure(EntityTypeBuilder<UserStudent> builder)
    {
        builder.ToTable("UserStudent", SchemaNames.Classes).IsMultiTenant();

    }
}

public class CommentConfig : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.ToTable("Comments", SchemaNames.Classes).IsMultiTenant();

    }
}

public class LikeConfig : IEntityTypeConfiguration<CommentLikes>
{
    public void Configure(EntityTypeBuilder<CommentLikes> builder)
    {
        builder.HasKey(b => new { b.CommentId, b.UserId});
        builder.ToTable("CommentLikes", SchemaNames.Classes).IsMultiTenant();
    }
}

