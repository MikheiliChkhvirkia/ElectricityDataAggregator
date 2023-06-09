using ElectricityDataAggregator.Common.Domain.Entities;

namespace ElectricityDataAggregator.Domain.Entities
{
    public class AggregatedData : TrackedEntity<int>
    {
        public string Region { get; set; }
        public double PPlusSum { get; set; }
        public double PMinusSum { get; set; }
    }
}
