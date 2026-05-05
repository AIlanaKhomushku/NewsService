using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewsService.Domain.NewsService.Domain;
using NewsService.Domain.NewsService.Domain.Enums;
using NewsService.Domain.NewsService.ValueObjects.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsService.Infrastructure.EntityFramework.Configurations;

public class NewsConfiguration : IEntityTypeConfiguration<News>
{
    public void Configure(EntityTypeBuilder<News> builder)
    {
        builder.ToTable("News");
        builder.HasKey(n => n.Id);

        builder.Property(n => n.Title)
            .IsRequired()
            .HasConversion(t => t.Value, str => new Title(str))
            .HasMaxLength(TitleValidator.MAX_LENGTH);

        builder.Property(n => n.Content)
            .IsRequired()
            .HasConversion(c => c.Value, str => new Content(str))
            .HasMaxLength(ContentValidator.MAX_LENGTH);

        builder.Property(n => n.NewsStatus)
            .HasDefaultValue(NewsStatus.Created)
            .HasConversion<string>();

        builder.Property(n => n.CreationData).IsRequired();

        builder.Property(n => n.ModificationData).IsRequired(false);

        builder.Property(n => n.Reactions)
            .HasConversion(r => r.ToString(), str => ReactionSummery.FromString(str));

        builder.HasOne(n => n.Author)
            .WithMany("_newss")
            .HasForeignKey("AuthorId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany<Comment>("_comments")
            .WithOne(c => c.News)
            .HasForeignKey("NewsId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation("_comments").SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}