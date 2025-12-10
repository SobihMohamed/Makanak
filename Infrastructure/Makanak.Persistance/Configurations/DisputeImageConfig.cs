using Makanak.Domain.Models.DisputeEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makanak.Persistance.Configurations
{
    public class DisputeImageConfig : IEntityTypeConfiguration<DisputeImage>
    {
        public void Configure(EntityTypeBuilder<DisputeImage> builder)
        {
            builder.ToTable("DisputeImages");
            builder.Property(di => di.ImageUrl).IsRequired().HasMaxLength(1000);
        }
    }
}
