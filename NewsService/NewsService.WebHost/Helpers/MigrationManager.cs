using Microsoft.EntityFrameworkCore;
namespace NewsService.WebHost.Helpers;

public static class MigrationManager
{
    public static IHost MigrateDatabase<T>(this IHost host) where T : DbContext
    {
        using var scope = host.Services.CreateScope();
        var appContext = scope.ServiceProvider.GetRequiredService<T>();
        appContext.Database.Migrate();
        return host;
    }
}