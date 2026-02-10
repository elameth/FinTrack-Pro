using FinTrackPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinTrackPro.Infrastructure.Persistence.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.AccountType)
            .IsRequired();

        builder.OwnsOne(a => a.Balance, balanceBuilder =>
        {
            balanceBuilder.Property(m => m.Amount)
                .HasColumnName("BalanceAmount")
                .HasPrecision(18, 2)
                .IsRequired();

            balanceBuilder.Property(m => m.Currency)
                .HasColumnName("BalanceCurrency")
                .IsRequired();
        });

        builder.Property(a => a.UserId)
            .IsRequired();

        builder.Property(a => a.CreatedAt)
            .IsRequired();

        builder.Property(a => a.UpdatedAt);

        builder.Property(a => a.IsActive)
            .IsRequired();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
