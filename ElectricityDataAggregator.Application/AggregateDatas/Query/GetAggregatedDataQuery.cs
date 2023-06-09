using ElectricityDataAggregator.Domain.Entities;
using MediatR;

namespace ElectricityDataAggregator.Application.AggregateDatas.Query
{
    public class GetAggregatedDataQuery : IRequest<GetAggregatedDataQueryResponse>
    { }

    public class GetAggregatedDataQueryResponse
    {
        public List<AggregatedData> Data { get; set; }
        public TimeSpan EstimateTime { get; set; }
        public string MemoryUsed { get; set; } 
    }

    public class AggregatedData 
    {
        public string Region { get; set; }
        public double PPlusSum { get; set; }
        public double PMinusSum { get; set; }
    }

    public class ElectricityDataEntry
    {
        public string Tinklas { get; set; }
        public string ObtPavadinimas { get; set; }
        public string ObjGvTipas { get; set; }
        public string ObjNumeris { get; set; }
        public double PPlus { get; set; }
        public string PlT { get; set; }
        public double PMinus { get; set; }
    }

}
