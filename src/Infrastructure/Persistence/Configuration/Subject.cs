using Finbuckle.MultiTenant.EntityFrameworkCore;
using FSH.WebApi.Domain.Subjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSH.WebApi.Infrastructure.Persistence.Configuration;
public class SubjectConfig : IEntityTypeConfiguration<Subject>
{
    public void Configure(EntityTypeBuilder<Subject> builder)
    {
        builder
              .ToTable("Subject", SchemaNames.Subject)
              .IsMultiTenant();

        builder
            .Property(b => b.Name)
                .HasMaxLength(256);
    }
}