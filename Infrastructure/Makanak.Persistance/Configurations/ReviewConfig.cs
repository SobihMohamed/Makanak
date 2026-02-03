using Makanak.Domain.Models.ReviewEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makanak.Persistance.Configurations
{
    public class ReviewConfig : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.ToTable("Reviews");
            builder.Property(r => r.Rating).IsRequired();
            builder.Property(r => r.Comment).HasMaxLength(1000);

            // booking has one review 
            builder.HasOne(r => r.Booking)
                .WithOne(b => b.Review)
                .HasForeignKey<Review>(r => r.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            // property can have many reviews
            builder.HasOne(r => r.Property)
                .WithMany(p=>p.Reviews)
                .HasForeignKey(r =>r.PropertyId)
                .OnDelete(DeleteBehavior.NoAction);

            // tenant can write many reviews
            builder.HasOne(r => r.Tenant)
                .WithMany(t => t.Reviews)
                .HasForeignKey(r => r.TenantId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
