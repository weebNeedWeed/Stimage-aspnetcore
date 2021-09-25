using imagestore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace imagestore.Data.Configurations
{
    public class AppImageConfiguration : IEntityTypeConfiguration<AppImage>
    {
        public void Configure(EntityTypeBuilder<AppImage> builder)
        {
            builder.HasKey(o => o.ImageId);
            builder.HasOne(o => o.AppUser)
                .WithMany(o => o.AppImages)
                .HasForeignKey(o => o.AppUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(o => o.CreatedAt).IsRequired().HasDefaultValue<DateTime>(DateTime.Now);

            builder.Property(o => o.IsPublic).HasDefaultValue<bool?>(true);

            builder.HasIndex(o => o.Slug).IsUnique();
        }     
    }
}
