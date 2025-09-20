using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fcg.Infrastructure.Tables.Mapping
{
    public class PromotionMapping : IEntityTypeConfiguration<Promotion>
    {
        public void Configure(EntityTypeBuilder<Promotion> builder)
        {
            builder.ToTable("Promotions");

            builder.HasKey(g => g.Id);
            builder.Property(g => g.Id)
                .ValueGeneratedNever()
                .IsRequired();

            builder.Property(p => p.Title)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Description)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(p => p.DiscountPercent)
                .IsRequired();

            builder.Property(p => p.StartDate)
                .IsRequired();

            builder.Property(p => p.EndDate)
                .IsRequired();

            builder.Property(p => p.Genre)
                .IsRequired();

            builder.ToTable(t => t.HasCheckConstraint("CK_Promotions_DiscountPercent_0_100",
               "\"DiscountPercent\" >= 0 AND \"DiscountPercent\" <= 100"));
        }
    }
}
