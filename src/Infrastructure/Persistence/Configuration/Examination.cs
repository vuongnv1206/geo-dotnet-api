using Finbuckle.MultiTenant.EntityFrameworkCore;
using FSH.WebApi.Domain.Examination;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Infrastructure.Persistence.Configuration;
public class PaperFolderConfig : IEntityTypeConfiguration<PaperFolder>
{
    public void Configure(EntityTypeBuilder<PaperFolder> builder)
    {
        builder.IsMultiTenant();
        builder.ToTable("PaperFolders", SchemaNames.Examination);
    }
}

public class PaperConfig : IEntityTypeConfiguration<Paper>
{
    public void Configure(EntityTypeBuilder<Paper> builder)
    {
        builder.IsMultiTenant();
        builder.ToTable("Papers", SchemaNames.Examination);
    }
}

public class PaperFolderPermissionConfig : IEntityTypeConfiguration<PaperFolderPermission>
{
    public void Configure(EntityTypeBuilder<PaperFolderPermission> builder)
    {
        builder.IsMultiTenant();
        builder.ToTable("PaperFolderPermissions", SchemaNames.Examination);
    }
}

public class PaperLabelConfig : IEntityTypeConfiguration<PaperLable>
{
    public void Configure(EntityTypeBuilder<PaperLable> builder)
    {
        builder.IsMultiTenant();
        builder.ToTable("PaperLabels", SchemaNames.Examination);
    }
}
