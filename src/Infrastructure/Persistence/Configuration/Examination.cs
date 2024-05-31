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

public class PaperLabelConfig : IEntityTypeConfiguration<PaperLabel>
{
    public void Configure(EntityTypeBuilder<PaperLabel> builder)
    {
        builder.IsMultiTenant();
        builder.ToTable("PaperLabels", SchemaNames.Examination);
    }
}

public class PaperQuestionConfig : IEntityTypeConfiguration<PaperQuestion>
{
    public void Configure(EntityTypeBuilder<PaperQuestion> builder)
    {
        builder.IsMultiTenant();
        builder.HasKey(tig => new { tig.PaperId, tig.QuestionId });
        builder.ToTable("PaperQuestions", SchemaNames.Examination);
    }
}

public class SubmitPaperConfig : IEntityTypeConfiguration<SubmitPaper>
{
    public void Configure(EntityTypeBuilder<SubmitPaper> builder)
    {
        builder.IsMultiTenant();
        builder.ToTable("SubmitPapers", SchemaNames.Examination);
    }
}

public class SubmitPaperDetailConfig : IEntityTypeConfiguration<SubmitPaperDetail>
{
    public void Configure(EntityTypeBuilder<SubmitPaperDetail> builder)
    {
        builder.IsMultiTenant();
        builder.HasKey(tig => new { tig.SubmitPaperId, tig.QuestionId });
        builder.ToTable("SubmitPaperDetails", SchemaNames.Examination);
    }
}
