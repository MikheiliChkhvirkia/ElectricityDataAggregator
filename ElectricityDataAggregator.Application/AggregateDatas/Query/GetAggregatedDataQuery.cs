using ElectricityDataAggregator.Domain.Entities;
using MediatR;

namespace ElectricityDataAggregator.Application.AggregateDatas.Query
{
    public class GetAggregatedDataQuery : IRequest<GetAggregatedDataQueryResponse>
    { }

    public class GetAggregatedDataQueryResponse
    {
        public List<AggregatedDataModel> Data { get; set; }
    }

    public class AggregatedDataModel 
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
