namespace UrlShortner.Database.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<UrlShortner.Database.ShortnerContext>
    {
        public Configuration()
        {
            //AutomaticMigrationsEnabled = false;
            //ContextKey = "Database.ShortnerContext";
        }

        protected override void Seed(UrlShortner.Database.ShortnerContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
        }
    }
}
