using Finbuckle.MultiTenant.EntityFrameworkCore;
using FSH.WebApi.Domain.Examination;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSH.WebApi.Infrastructure.Persistence.Configuration;
public class PaperFolderConfig : IEntityTypeConfiguration<PaperFolder>
{
    public void Configure(EntityTypeBuilder<PaperFolder> builder)
    {
        _ = builder.IsMultiTenant();
        _ = builder.ToTable("PaperFolders", SchemaNames.Examination);
    }
}

public class PaperConfig : IEntityTypeConfiguration<Paper>
{
    public void Configure(EntityTypeBuilder<Paper> builder)
    {
        _ = builder.IsMultiTenant();
        _ = builder.ToTable("Papers", SchemaNames.Examination);
    }
}

public class PaperFolderPermissionConfig : IEntityTypeConfiguration<PaperFolderPermission>
{
    public void Configure(EntityTypeBuilder<PaperFolderPermission> builder)
    {
        _ = builder.IsMultiTenant();
        _ = builder.ToTable("PaperFolderPermissions", SchemaNames.Examination);
    }
}

public class PaperLabelConfig : IEntityTypeConfiguration<PaperLabel>
{
    public void Configure(EntityTypeBuilder<PaperLabel> builder)
    {
        _ = builder.IsMultiTenant();
        _ = builder.ToTable("PaperLabels", SchemaNames.Examination);
    }
}

public class PaperQuestionConfig : IEntityTypeConfiguration<PaperQuestion>
{
    public void Configure(EntityTypeBuilder<PaperQuestion> builder)
    {
        _ = builder.IsMultiTenant();
        _ = builder.HasKey(tig => new { tig.PaperId, tig.QuestionId });
        _ = builder.ToTable("PaperQuestions", SchemaNames.Examination);
    }
}

public class SubmitPaperConfig : IEntityTypeConfiguration<SubmitPaper>
{
    public void Configure(EntityTypeBuilder<SubmitPaper> builder)
    {
        _ = builder.IsMultiTenant();
        _ = builder.ToTable("SubmitPapers", SchemaNames.Examination);
    }
}

public class SubmitPaperDetailConfig : IEntityTypeConfiguration<SubmitPaperDetail>
{
    public void Configure(EntityTypeBuilder<SubmitPaperDetail> builder)
    {
        _ = builder.IsMultiTenant();
        _ = builder.ToTable("SubmitPaperDetails", SchemaNames.Examination);
    }
}

public class SubmitPaperLogConfig : IEntityTypeConfiguration<SubmitPaperLog>
{
    public void Configure(EntityTypeBuilder<SubmitPaperLog> builder)
    {
        _ = builder.IsMultiTenant();
        _ = builder.ToTable("SubmitPaperLogs", SchemaNames.Examination);
    }
}

public class PaperAccessQuestionConfig : IEntityTypeConfiguration<PaperAccess>
{
    public void Configure(EntityTypeBuilder<PaperAccess> builder)
    {
        _ = builder.IsMultiTenant();
        _ = builder.ToTable("PaperAccesses", SchemaNames.Examination);
    }
}

public class PaperPermissionConfig : IEntityTypeConfiguration<PaperPermission>
{
    public void Configure(EntityTypeBuilder<PaperPermission> builder)
    {
        _ = builder.IsMultiTenant();
        _ = builder.ToTable("PaperPermissions", SchemaNames.Examination);
    }
}

public class PaperMatrixConfig : IEntityTypeConfiguration<PaperMatrix>
{
    public void Configure(EntityTypeBuilder<PaperMatrix> builder)
    {
        _ = builder.IsMultiTenant();
        _ = builder.ToTable("PaperMatrices", SchemaNames.Examination);
    }
}
