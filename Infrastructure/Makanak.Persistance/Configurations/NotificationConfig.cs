using Makanak.Domain.Models.NotifyEnities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makanak.Persistance.Configurations
{
    public class NotificationConfig : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("Notifications");
            builder.Property(n => n.Title).IsRequired().HasMaxLength(200);
            builder.Property(n => n.Message).IsRequired().HasMaxLength(1000);
            builder.Property(n => n.NotificationType).HasConversion<string>();

            builder.HasOne(n=>n.ApplicationUser)
                     .WithMany(u=>u.Notifications)
                     .HasForeignKey(n=>n.UserId)
                     .OnDelete(DeleteBehavior.Cascade);

            // indexes to optimization search
            builder.HasIndex(n => n.UserId); 
            builder.HasIndex(n => n.IsRead); 
        }
    }
}
