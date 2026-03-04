using Makanak.Domain.Models.ResetPassword;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Persistance.Configurations
{
    public class UserOtpConfig : IEntityTypeConfiguration<UserOtp>
    {
        public void Configure(EntityTypeBuilder<UserOtp> builder)
        {
            builder.ToTable("UserOtps");
            builder.Property(u => u.Email).IsRequired().HasMaxLength(256);
            builder.Property(u => u.OtpCode).IsRequired().HasMaxLength(10);
            builder.Property(u => u.ExpirationTime).IsRequired();
            builder.HasOne(u => u.User)
                   .WithMany()
                   .HasForeignKey(u => u.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.HasIndex(u => u.Email);
        }
    }
}
