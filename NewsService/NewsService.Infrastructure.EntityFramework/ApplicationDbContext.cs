using Microsoft.EntityFrameworkCore;
using NewsService.Domain.NewsService.Domain;

namespace NewsService.Infrastructure.EntityFramework;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<News> Newss { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Reaction> Reactions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.EnableSensitiveDataLogging();
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }


}
