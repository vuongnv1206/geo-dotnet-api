using Finbuckle.MultiTenant.EntityFrameworkCore;
using FSH.WebApi.Domain.Class;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSH.WebApi.Infrastructure.Persistence.Configuration;

public class ClassesConfig : IEntityTypeConfiguration<Classes>
{
    public void Configure(EntityTypeBuilder<Classes> builder)
    {
        builder.ToTable("Classes", SchemaNames.Classroom).IsMultiTenant();

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
        builder.ToTable("GroupClasses", SchemaNames.Classroom).IsMultiTenant();
    }
}

public class PostConfig : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.ToTable("Post", SchemaNames.Classroom).IsMultiTenant();
    }
}

public class PostLikeConfig : IEntityTypeConfiguration<PostLike>
{
    public void Configure(EntityTypeBuilder<PostLike> builder)
    {
        builder.IsMultiTenant();
        builder.HasKey(b => new { b.UserId, b.PostId });
        builder.ToTable("PostLike", SchemaNames.Classroom);
    }
}

public class UserClassConfig : IEntityTypeConfiguration<UserClass>
{
    public void Configure(EntityTypeBuilder<UserClass> builder)
    {
        builder.IsMultiTenant();
        builder.HasKey(b => new { b.ClassesId, b.StudentId });
        builder.ToTable("UserClasses", SchemaNames.Classroom);
    }
}

public class StudentConfig : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("Student", SchemaNames.Classroom).IsMultiTenant();
    }
}

public class CommentConfig : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.ToTable("Comments", SchemaNames.Classroom).IsMultiTenant();
    }
}

public class CommentLikesConfig : IEntityTypeConfiguration<CommentLikes>
{
    public void Configure(EntityTypeBuilder<CommentLikes> builder)
    {
        builder.HasKey(b => new { b.CommentId, b.UserId});
        builder.ToTable("CommentLikes", SchemaNames.Classroom).IsMultiTenant();
    }
}

