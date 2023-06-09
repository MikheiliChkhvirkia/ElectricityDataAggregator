using ElectricityDataAggregator.Application.Infrastructure.Persistance;
using MediatR;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;

namespace ElectricityDataAggregator.Application.AggregateDatas.Query
{
    public class GetAggregatedDataQueryHandler : IRequestHandler<GetAggregatedDataQuery, GetAggregatedDataQueryResponse>
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private readonly IElectricityDbContext db;

        public GetAggregatedDataQueryHandler(IElectricityDbContext db)
        {
            this.db = db;
        }

        public async Task<GetAggregatedDataQueryResponse> Handle(GetAggregatedDataQuery request, CancellationToken cancellationToken)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            var urls = new List<string>
            {
                "https://data.gov.lt/dataset/1975/download/10766/2022-05.csv",
                "https://data.gov.lt/dataset/1975/download/10765/2022-04.csv",
                "https://data.gov.lt/dataset/1975/download/10764/2022-03.csv",
                "https://data.gov.lt/dataset/1975/download/10763/2022-02.csv"
            };

            var aggregatedData = new ConcurrentDictionary<string, (double PPlus, double PMinus)>();

            await Task.WhenAll(urls.Select(url => ProcessCsvData(url, aggregatedData)));

            await StoreAggregatedDataInDatabase(aggregatedData, cancellationToken);

            var response = new GetAggregatedDataQueryResponse
            {
                Data = aggregatedData.Select(entry => new AggregatedData
                {
                    Region = entry.Key,
                    PPlusSum = entry.Value.PPlus,
                    PMinusSum = entry.Value.PMinus
                }).ToList()
            };

            stopwatch.Stop();
            response.EstimateTime = stopwatch.Elapsed;

            return response;
        }

        private static async Task ProcessCsvData(string url, ConcurrentDictionary<string, (double PPlus, double PMinus)> aggregatedData)
        {
            using var responseStream = await httpClient.GetStreamAsync(url);
            using var reader = new StreamReader(responseStream);

            // Skip the header row
            await reader.ReadLineAsync();

            string line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                var values = line.Split(',');

                var obtPavadinimas = values[1];

                if (obtPavadinimas.Equals("Butas"))
                {
                    var entry = new ElectricityDataEntry
                    {
                        Tinklas = values[0],
                        ObtPavadinimas = obtPavadinimas,
                        ObjGvTipas = values[2],
                        ObjNumeris = values[3],
                        PPlus = ParseDoubleOrDefault(values[4]),
                        PlT = values[5],
                        PMinus = ParseDoubleOrDefault(values[6])
                    };

                    aggregatedData.AddOrUpdate(
                        entry.Tinklas,
                        (entry.PPlus, entry.PMinus),
                        (_, existingEntry) => (existingEntry.PPlus + entry.PPlus, existingEntry.PMinus + entry.PMinus)
                    );
                }
            }
        }

        private static double ParseDoubleOrDefault(string value)
        {
            if (double.TryParse(value?.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
            {
                return result;
            }

            // Parsing failed or value is null. Using default value
            return default;
        }

        private async Task StoreAggregatedDataInDatabase(ConcurrentDictionary<string, (double PPlus, double PMinus)> aggregatedData, CancellationToken cancellationToken)
        {
            var records = aggregatedData.Select(entry => new Domain.Entities.AggregatedData
            {
                Region = entry.Key,
                PPlusSum = entry.Value.PPlus,
                PMinusSum = entry.Value.PMinus
            }).ToList();

            await db.AggregatedDatas.AddRangeAsync(records, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);
        }
    }
}
