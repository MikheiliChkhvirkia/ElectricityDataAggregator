using Microsoft.EntityFrameworkCore;

namespace ElectricityDataAggregator.Common.Persistence
{
    public class DBContextBase : DbContext
    {
        public DBContextBase(DbContextOptions options)
            : base(options) { }
    }
}