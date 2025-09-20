using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fcg.Infrastructure.Tables.Mapping
{
    public class UserGamingMapping : IEntityTypeConfiguration<UserGaming>
    {
        public void Configure(EntityTypeBuilder<UserGaming> builder)
        {
            builder.ToTable("UserGamings"); ;

            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id)
                .ValueGeneratedNever()
                .IsRequired();

            builder.Property(p => p.PurchasedDate).IsRequired();

            builder.HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(fk => fk.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.Game)
                .WithMany()
                .HasForeignKey(fk => fk.GameId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
