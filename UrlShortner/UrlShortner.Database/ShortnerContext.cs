

using System.Data.Entity;
using UrlShortner.Database.Entities;

namespace UrlShortner.Database
{
    public class ShortnerContext : DbContext
    {
        public ShortnerContext() : base("name=UrlShortnerContext")
        {
            //System.Data.Entity.Database.SetInitializer(new MigrateDatabaseToLatestVersion<ShortnerContext, UrlShortner.Database.Migrations.Configuration>());
           // System.Data.Entity.Database.SetInitializer<ShortnerContext>(new CreateDatabaseIfNotExists<ShortnerContext>());
        }
        public DbSet<AdManage> AdManage { get; set; }
        public DbSet<UrlShortnerHistory> UrlShortnerHistory { get; set; }
        public DbSet<Database.Entities.UrlShortner> UrlShortner { get; set; }
        public DbSet<Registration> Registration { get; set; }
        public DbSet<PaymentTransaction> PaymentTransaction { get; set; }
        public DbSet<UrlVisitorLog> UrlVisitorLog { get; set; }
        public DbSet<ShortCharacters> ShortCharacters { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
          //  base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Registration>()
                .HasMany(t => t.AdManages)
                .WithRequired(a => a.Registration)
                .WillCascadeOnDelete(false);
        }

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    Database.SetInitializer<UrlShortnerContext>(null);
        //    base.OnModelCreating(modelBuilder);
        //}



    }
}
