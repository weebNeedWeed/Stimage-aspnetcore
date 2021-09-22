using imagestore.Data.Configurations;
using imagestore.Extensions;
using imagestore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace imagestore.Data
{
    public class StimageContext : IdentityDbContext<AppUser>
    {
        public StimageContext(DbContextOptions<StimageContext> options): base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                if (tableName.StartsWith("AspNet"))
                {
                    entityType.SetTableName("App" + tableName.Substring(6));
                }
            }

            modelBuilder.ApplyConfiguration(new AppImageConfiguration());

            modelBuilder.Seed();
        }

        public DbSet<AppImage> AppImages { get; set; }
    }
}
