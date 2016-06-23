namespace MyServer.Web.Api
{
    using System.Data.Entity;

    using MyServer.Data;
    using MyServer.Data.Migrations;

    public static class DatabaseConfig
    {
        public static void Initialize()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<MyServerDbContext, Configuration>());
        }
    }
}