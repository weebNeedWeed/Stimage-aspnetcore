using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;

namespace imagestore.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            var adminRoleId = Guid.NewGuid().ToString();
            var defaultRoleId = Guid.NewGuid().ToString();

            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = adminRoleId,
                    Name = "admin",
                    NormalizedName = "admin".ToUpper(),
                },
                new IdentityRole
                {
                    Id = defaultRoleId,
                    Name = "default",
                    NormalizedName = "default".ToUpper(),
                }
                );
        }
    }
}
