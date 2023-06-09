using ElectricityDataAggregator.Application.AggregateDatas.Query;
using ElectricityDataAggregator.Application.Infrastructure.Persistance;
using ElectricityDataAggregator.Domain.Entities;
using ElectricityDataAggregator.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Collections.Concurrent;

namespace ElectricityDataAggregator.Application.Tests.ElectricityAggregation.Query
{
    public class GetElectricityAggregationQueryHandlerTests : IClassFixture<ElectricityAggregationFixture>
    {
        public readonly ElectricityAggregationFixture fixture;
        public readonly IElectricityDbContext db;

        public GetElectricityAggregationQueryHandlerTests(ElectricityAggregationFixture fixture)
        {
            this.fixture = fixture;
            db = new ElectricityDbContext(new DbContextOptionsBuilder<ElectricityDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            this.fixture.ServiceCollection.AddScoped(_ => db);
        }

        [Fact]
        public async Task Handle_Should_Return_AggregatedDataQueryResponse()
        {
            // Arrange
            var cancellationToken = new CancellationToken();
            var mockDb = new Mock<IElectricityDbContext>();
            var mockConfiguration = new Mock<IConfiguration>();

            var handler = new GetAggregatedDataQueryHandler(mockDb.Object, mockConfiguration.Object);
            var query = new GetAggregatedDataQuery();

            var aggregatedData = new ConcurrentDictionary<string, (double PPlus, double PMinus)>();
            aggregatedData.TryAdd("Region1", (400, 200));
            aggregatedData.TryAdd("Region2", (800, 400));

            var expectedResponse = new GetAggregatedDataQueryResponse
            {
                Data = new List<AggregatedDataModel>
                {
                    new AggregatedDataModel
                    {
                        Region = "Region1",
                        PPlusSum = 400,
                        PMinusSum = 200
                    },
                    new AggregatedDataModel
                    {
                        Region = "Region2",
                        PPlusSum = 800,
                        PMinusSum = 400
                    }
                }
            };

            mockConfiguration.Setup(c => c["CsvFilePathFormat"]).Returns("C:\\Users\\Mishiko\\source\\repos\\ElectricityDataAggregator\\ElectricityDataAggregator\\csvFiles\\2022-02-test.csv");
            mockDb.Setup(d => d.AggregatedDatas.AddRangeAsync(It.IsAny<List<AggregatedData>>(), cancellationToken)).Verifiable();
            mockDb.Setup(d => d.SaveChangesAsync(cancellationToken)).ReturnsAsync(1);

            // Act
            var response = await handler.Handle(query, cancellationToken);

            // Assert
            response.Should().BeEquivalentTo(expectedResponse);
            mockDb.Verify(d => d.AggregatedDatas.AddRangeAsync(It.IsAny<List<AggregatedData>>(), cancellationToken), Times.Once);
            mockDb.Verify(d => d.SaveChangesAsync(cancellationToken), Times.Once);
        }
    }
}
