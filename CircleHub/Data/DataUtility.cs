using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Bogus;
using CircleHub.Models;

namespace CircleHub.Data;

public class DataUtility
{
    public static string GetConnectionString(IConfiguration config)
    {
        var connectionString = config.GetConnectionString("DbConnection"); //Local connection string
        var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_PRIVATE_URL"); //Railway connection string

        return string.IsNullOrEmpty(databaseUrl) ? connectionString! : BuildConnectionString(databaseUrl);
    }

    private static string BuildConnectionString(string databaseUrl)
    {
        var databaseUri = new Uri(databaseUrl);
        var userInfo = databaseUri.UserInfo.Split(':');
        var builder = new Npgsql.NpgsqlConnectionStringBuilder
        {
            Host = databaseUri.Host,
            Port = databaseUri.Port,
            Username = userInfo[0],
            Password = userInfo[1],
            Database = databaseUri.LocalPath.TrimStart('/'),
            SslMode = Npgsql.SslMode.Require,
        };
        return builder.ToString();
    }

    public static async Task ManageDataAsync(IServiceProvider svcProvider)
    {
        var dbContextSvc = svcProvider.GetRequiredService<ApplicationDbContext>();
        var userManagerSvc = svcProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var configSvc = svcProvider.GetRequiredService<IConfiguration>();

        await dbContextSvc.Database.MigrateAsync();
    }
}
