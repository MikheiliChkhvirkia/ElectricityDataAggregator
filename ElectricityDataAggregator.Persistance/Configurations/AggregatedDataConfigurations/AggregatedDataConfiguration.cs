using ElectricityDataAggregator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectricityDataAggregator.Persistance.Configurations.AggregatedDataConfigurations
{
    public class AggregatedDataConfiguration : IEntityTypeConfiguration<AggregatedData>
    {
        public void Configure(EntityTypeBuilder<AggregatedData> builder)
        {
            builder.ToTable("AggregatedData");
        }
    }
}
