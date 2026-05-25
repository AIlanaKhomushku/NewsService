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

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Username)
            .IsRequired()
            .HasConversion(username => username.Value, str => new Username(str))
            .HasMaxLength(UsernameValidator.MAX_LENGTH);

        builder.HasMany<Comment>("Comments")
            .WithOne(c => c.User)
            .HasForeignKey("UserId")
            .OnDelete(DeleteBehavior.Cascade);
        builder.Metadata.FindNavigation(nameof(User.Comments))?.SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany<Reaction>(u => u.Reactions)
            .WithOne(r => r.User)       
            .HasForeignKey("UserId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation(nameof(User.Reactions))?.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}