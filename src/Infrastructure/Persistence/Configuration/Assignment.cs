using Finbuckle.MultiTenant.EntityFrameworkCore;
using FSH.WebApi.Domain.Assignment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSH.WebApi.Infrastructure.Persistence.Configuration;

public class AssignmentConfig : IEntityTypeConfiguration<Assignment>
{
    public void Configure(EntityTypeBuilder<Assignment> builder)
    {
        builder
              .ToTable("Assignment", SchemaNames.Assignment)
              .IsMultiTenant();

        builder
            .Property(b => b.Name)
                .HasMaxLength(256);

        builder
          .Property(p => p.AttachmentPath)
              .HasMaxLength(2048);
    }

    public class AssignmentClassConfig : IEntityTypeConfiguration<AssignmentClass>
    {
        public void Configure(EntityTypeBuilder<AssignmentClass> builder)
        {
            builder
                  .HasKey(x => new { x.AssignmentId, x.ClassesId });

            builder
                  .ToTable("AssignmentClass", SchemaNames.Assignment)
                  .IsMultiTenant();
        }

    }

    public class AssignmentStudentConfig : IEntityTypeConfiguration<AssignmentStudent>
    {
        public void Configure(EntityTypeBuilder<AssignmentStudent> builder)
        {

            builder
                  .HasKey(x => new { x.AssignmentId, x.StudentId });
            builder
                  .ToTable("AssignmentStudent", SchemaNames.Assignment)
                  .IsMultiTenant();

            builder
              .Property(p => p.AttachmentPath)
                  .HasMaxLength(2048);
        }
    }
}

