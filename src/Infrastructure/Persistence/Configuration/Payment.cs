using Finbuckle.MultiTenant.EntityFrameworkCore;
using FSH.WebApi.Domain.Payment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSH.WebApi.Infrastructure.Persistence.Configuration;
public class OrderConfig : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder
            .ToTable("Orders", SchemaNames.Payment)
            .IsMultiTenant();

        builder.Property(x => x.OrderNo)
            .HasMaxLength(50);

        builder.HasOne(x => x.Subscription)
            .WithMany()
            .HasForeignKey(x => x.SupscriptionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class SubscriptionConfig : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder
            .ToTable("Subscriptions", SchemaNames.Payment)
            .IsMultiTenant();

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();
    }
}

public class TransactionConfig : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder
            .ToTable("Transactions", SchemaNames.Payment)
            .IsMultiTenant();
    }
}
