using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fcg.Infrastructure.Tables.Mapping
{
    public class GameMapping : IEntityTypeConfiguration<Game>
    {
        public void Configure(EntityTypeBuilder<Game> builder)
        {
            builder.ToTable("Games");

            builder.HasKey(g => g.Id);
            builder.Property(g => g.Id)
                .ValueGeneratedNever()
                .IsRequired();

            builder.Property(g => g.Title)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(g => g.Description)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(g => g.Genre)
                .IsRequired();

            builder.Property(g => g.CreatedAt)
                .IsRequired();

            builder.Property(g => g.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)");
        }
    }
}
