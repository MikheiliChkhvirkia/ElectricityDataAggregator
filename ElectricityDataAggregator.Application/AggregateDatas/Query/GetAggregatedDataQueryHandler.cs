using ElectricityDataAggregator.Application.Infrastructure.Persistance;
using MediatR;
using Microsoft.Extensions.Configuration;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;

namespace ElectricityDataAggregator.Application.AggregateDatas.Query
{
    public class GetAggregatedDataQueryHandler : IRequestHandler<GetAggregatedDataQuery, GetAggregatedDataQueryResponse>
    {
        private readonly IElectricityDbContext db;
        private readonly IConfiguration configuration;
        private readonly Stopwatch stopwatch = Stopwatch.StartNew();

        public GetAggregatedDataQueryHandler(IElectricityDbContext db, IConfiguration configuration)
        {
            this.db = db;
            this.configuration = configuration;
        }

        public async Task<GetAggregatedDataQueryResponse> Handle(GetAggregatedDataQuery request, CancellationToken cancellationToken)
        {

            var aggregatedData = new ConcurrentDictionary<string, (double PPlus, double PMinus)>();
            var rootPath = GetRootPath(); // Get the root path for CSV files
            var csvFilePaths = GetCsvFilePaths(rootPath); // Get the file paths of the CSV files

            await ProcessCsvData(csvFilePaths, aggregatedData); // Process the CSV files

            await StoreAggregatedDataInDatabase(aggregatedData, cancellationToken); // Store the aggregated data in the database

            stopwatch.Stop();

            return new GetAggregatedDataQueryResponse
            {
                Data = aggregatedData.Select(entry => new AggregatedData
                {
                    Region = entry.Key,
                    PPlusSum = entry.Value.PPlus,
                    PMinusSum = entry.Value.PMinus
                }).ToList(),
                EstimateTime = stopwatch.Elapsed, //Stopwatch timer
                MemoryUsed = FormatBytes(Process.GetCurrentProcess().WorkingSet64) //Get Memory and convert into readable format
            };
        }

        #region Private
        private string GetRootPath()
            => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configuration["CsvFilePathFormat"])
            .Replace("\\bin\\Debug\\net6.0", "");

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

        #region Statics
        private static string FormatBytes(long bytes)
        {
            const int scale = 1024;
            string[] sizeTextArr = { "TB", "GB", "MB", "KB", "Bytes" };
            long max = (long)Math.Pow(scale, sizeTextArr.Length - 1);

            foreach (string size in sizeTextArr)
            {
                if (bytes > max)
                    return $"{bytes / max} {size}";

                max /= scale;
            }

            return "0 Bytes";
        }

        // Generate the file paths of the CSV files for the desired months
        private static List<string> GetCsvFilePaths(string rootPath)
            => Enumerable.Range(2, 4)
               .Select(month => string.Format(rootPath, (month < 10) ? $"0{month}" : month.ToString()))
               .ToList();


        private static async Task ProcessCsvData(List<string> filePaths, ConcurrentDictionary<string, (double PPlus, double PMinus)> aggregatedData)
        {
            var tasks = new List<Task>();

            foreach (var filePath in filePaths)
            {
                tasks.Add(Task.Run(async () =>
                {
                    using var reader = new StreamReader(filePath);

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
                }));
            }
            // wait until all tasks are finished
            await Task.WhenAll(tasks);
        }

        //Get Parsed value or return default
        private static double ParseDoubleOrDefault(string value)
            => double.TryParse(value?.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var result)
                ? result
                : default;

        #endregion

        #endregion
    }
}
