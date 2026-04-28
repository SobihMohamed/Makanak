using Makanak.Domain.Models.BookingEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makanak.Persistance.Configurations
{
    public class BookingConfig : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.Property(b => b.CheckInDate).IsRequired();
            builder.Property(b => b.CheckOutDate).IsRequired();
            builder.Property(b => b.TotalDays).IsRequired();
            builder.Property(b => b.PricePerNight).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(b => b.TotalPrice).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(b => b.CommissionPaid).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(b => b.AmountToPayToOwner).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(b => b.Status).HasConversion<string>().IsRequired();
            builder.Property(b => b.NumberOfGuests).IsRequired();
            builder.Property(b => b.SpecialRequests).HasMaxLength(1000);
            builder.Property(b => b.CancellationReason).HasMaxLength(1000);
            builder.Property(b => b.CheckInQrCode).HasMaxLength(1000);
            builder.Property(b => b.RefundedAmount).HasColumnType("decimal(18,2)");

            // Booking has one Property but Property has many Bookings
            builder.HasOne(p=>p.Property)
                .WithMany(b=>b.Bookings)
                .HasForeignKey(b=>b.PropertyId)
                .OnDelete(DeleteBehavior.NoAction);
            // Booking has one Tenant but Tenant has many Bookings
            // دي مفروض الحجوزات اللي عملها المستأجر
            builder.HasOne(b => b.Tenant)
                .WithMany(t => t.TenantBookings)
                .HasForeignKey(b => b.TenantId)
                .OnDelete(DeleteBehavior.NoAction);
            // Booking has one Owner but Owner has many Bookings
            // دي مفروض الحجوزات اللي وصلت لصاحب الشقه 
            builder.HasOne(b => b.Owner)
                .WithMany(o => o.IncomingBookings)
                .HasForeignKey(b => b.OwnerId)
                .OnDelete(DeleteBehavior.NoAction);
            // Booking has many Disputes but Dispute has one Booking
            // configured in DisputeConfig

            // Booking has one Review and Review has one Booking
            // configured in ReviewConfig


        }
    }
}
