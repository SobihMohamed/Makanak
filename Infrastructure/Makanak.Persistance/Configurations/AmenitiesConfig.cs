using Makanak.Domain.Models.PropertyEntities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makanak.Persistance.Configurations
{
    public class AmenitiesConfig : IEntityTypeConfiguration<Amenity>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Amenity> builder)
        {
            builder.ToTable("Amenities");
            builder.Property(a => a.NameAr).IsRequired().HasMaxLength(100);
            builder.Property(a => a.NameEn).IsRequired().HasMaxLength(100);
            builder.Property(a => a.Icon).HasMaxLength(200);

        }
    }
}
