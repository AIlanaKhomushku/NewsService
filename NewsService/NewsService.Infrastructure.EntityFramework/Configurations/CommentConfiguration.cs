using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewsService.Domain.NewsService.Domain;
using NewsService.Domain.NewsService.ValueObjects.Validators;
using NewsService.Domain.NewsService.ValueObjects;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NewsService.Infrastructure.EntityFramework.Configurations;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.ToTable("Comments");
        builder.HasKey(c => c.Id);

        builder.Property<Guid>("NewsId").IsRequired();
        builder.Property<Guid>("UserId").IsRequired();

        builder.Property(c => c.Content)
            .IsRequired()
            .HasConversion(con => con.Value, str => new CommentText(str))
            .HasMaxLength(CommentTextValidator.MAX_LENGTH);

        builder.Property(c => c.CreationData).IsRequired();

        builder.HasOne(c => c.News)
            .WithMany(n => n.Comments)
            .HasForeignKey("NewsId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.User)
            .WithMany(u => u.Comments)
            .HasForeignKey("UserId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation(nameof(News.Comments))?.SetPropertyAccessMode(PropertyAccessMode.Field);
        builder.Metadata.FindNavigation(nameof(User.Comments))?.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}