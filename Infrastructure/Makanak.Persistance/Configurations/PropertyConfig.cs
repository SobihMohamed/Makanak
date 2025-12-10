using Makanak.Domain.Models.PropertyEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makanak.Persistance.Configurations
{
    public class PropertyConfig : IEntityTypeConfiguration<Property>
    {
        public void Configure(EntityTypeBuilder<Property> builder)
        {
            builder.ToTable("Properties");

            builder.Property(p => p.Title).IsRequired().HasMaxLength(200);
            builder.Property(p => p.Description).IsRequired().HasMaxLength(1000);
            builder.Property(p => p.Address).IsRequired().HasMaxLength(500);
            builder.Property(p => p.AreaName).IsRequired().HasMaxLength(100);
            builder.Property(p => p.MainImageUrl).IsRequired().HasMaxLength(150);



            builder.Property(p => p.PricePerNight).HasColumnType("decimal(18,2)");
            builder.Property(p => p.CommissionPercentage).HasColumnType("decimal(18,2)");

            builder.Property(p => p.PropertyType).HasConversion<string>();

            builder.HasOne(p=>p.Governorate)
                   .WithMany()
                   .HasForeignKey(p => p.GovernorateId)
                   .OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(o=>o.Owner)
                   .WithMany(p=>p.OwnedProperties)
                   .HasForeignKey(p => p.OwnerId)
                   .OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(img=>img.PropertyImages)
                   .WithOne(p => p.Property)
                   .HasForeignKey(pi => pi.PropertyId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(p=>p.Amenities)
                   .WithMany(a=>a.Properties)
                   .UsingEntity(j=>j.ToTable("PropertyAmenities"));
        }
    }
}
