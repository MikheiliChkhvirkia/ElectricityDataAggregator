using ElectricityDataAggregator.Application.Infrastructure.Persistance;
using ElectricityDataAggregator.Common.Domain.Contracts;
using ElectricityDataAggregator.Common.Persistence;
using ElectricityDataAggregator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Reflection;

namespace ElectricityDataAggregator.Persistence
{
    public class ElectricityDbContext : DBContextBase, IElectricityDbContext
    {

        public ElectricityDbContext(DbContextOptions<ElectricityDbContext> options)
    : base(options)
        { }
        protected void OnModelCreating(ModelBuilder modelBuilder, Assembly assembly)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(assembly);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries<ITrackedEntity>();
            var userId = "123";

            foreach (var entry in entries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.UpdateCreateCredentials(DateTime.Now, userId);
                        break;
                    case EntityState.Modified:
                        if (entry.Entity.DeleteDate > DateTime.MinValue)
                        {
                            entry.Entity.UpdateDeleteCredentials(DateTime.Now, userId);
                        }
                        else
                        {
                            entry.Entity.UpdateLastModifiedCredentials(DateTime.Now, userId);
                        }
                        break;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }

        #region DbSets
        public virtual DbSet<AggregatedData> AggregatedDatas { get; set; }

        #endregion


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
