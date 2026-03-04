
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
    public class DisputeConfig : IEntityTypeConfiguration<Dispute>
    {
        public void Configure(EntityTypeBuilder<Dispute> builder)
        {
            builder.ToTable("Disputes");
            builder.Property(d=>d.Reason).HasConversion<string>().IsRequired();
            builder.Property(d => d.Status).HasConversion<string>().IsRequired();
            builder.Property(d=>d.Description).HasMaxLength(1000);
            builder.Property(d=>d.AdminComment).HasMaxLength(1000);

            builder.HasOne(d => d.Booking)
                .WithMany(b=>b.Disputes)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(d=>d.Complainant)
                .WithMany(u=>u.Disputes)
                .HasForeignKey(d=>d.ComplainantId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(d=>d.DisputeImages)
                .WithOne(di=>di.Dispute)
                .HasForeignKey(di=>di.DisputeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
