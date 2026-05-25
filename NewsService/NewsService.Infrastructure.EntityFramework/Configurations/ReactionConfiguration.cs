using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewsService.Domain.NewsService.Domain;

namespace NewsService.Infrastructure.EntityFramework.Configurations;

public class ReactionConfiguration : IEntityTypeConfiguration<Reaction>
{
    public void Configure(EntityTypeBuilder<Reaction> builder)
    {
        builder.ToTable("Reactions");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.CreationDate).IsRequired();
        builder.Property(r => r.Type).IsRequired();

        builder.Property<Guid>("NewsId").IsRequired();
        builder.Property<Guid>("UserId").IsRequired();

        builder.HasOne(r => r.News)
            .WithMany(n => n.Reactions)
            .HasForeignKey("NewsId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.User)
            .WithMany(u => u.Reactions)
            .HasForeignKey("UserId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}