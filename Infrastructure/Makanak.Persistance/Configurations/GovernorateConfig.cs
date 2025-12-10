using Makanak.Domain.Models.LocationEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Makanak.Persistance.Configurations
{
    public class GovernorateConfig : IEntityTypeConfiguration<Governorate>
    {
        public void Configure(EntityTypeBuilder<Governorate> builder)
        {
            builder.ToTable("Governorates");
            builder.Property(g => g.NameEn).IsRequired().HasMaxLength(100);
            builder.Property(g => g.NameAr).IsRequired().HasMaxLength(100);
        }
    }
}
