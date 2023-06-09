using ElectricityDataAggregator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace ElectricityDataAggregator.Application.Infrastructure.Persistance
{
    public interface IElectricityDbContext
    {
        #region DbSet
        DbSet<AggregatedData> AggregatedDatas { get; set; }

        #endregion
        DatabaseFacade Database { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
