using Makanak.Domain.Models.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makanak.Persistance.Configurations
{
    public class ApplicationUserConfig : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.ToTable("Users");
            builder.Property(u => u.NegativeBalance).HasColumnType("Decimal(18,2)");
            builder.Property(e => e.Name).IsRequired().HasMaxLength(100);
            builder.Property(e => e.NationalId).HasColumnType("Char(14)");          
            builder.Property(u => u.UserType).HasConversion<string>();
            builder.Property(u => u.UserStatus).HasConversion<string>();
        }
    }
}
