using FSH.WebApi.Domain.Subjects;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Finbuckle.MultiTenant.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

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