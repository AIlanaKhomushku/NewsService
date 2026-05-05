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

namespace NewsService.Infrastructure.EntityFramework.Configurations;

public class AuthorConfiguration : IEntityTypeConfiguration<Author>
{
    public void Configure(EntityTypeBuilder<Author> builder)
    {
        builder.ToTable("Authors");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Authorname)
            .IsRequired()
            .HasConversion(an => an.Value, str => new Authorname(str))
            .HasMaxLength(AuthornameValidator.MAX_LENGTH);

        builder.HasMany <News>("_newss")
            .WithOne(n => n.Author)
            .HasForeignKey("AuthorId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation("_newss")?.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
