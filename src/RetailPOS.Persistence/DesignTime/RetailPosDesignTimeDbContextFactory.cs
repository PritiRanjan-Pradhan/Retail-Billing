using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using RetailPOS.Persistence.Contexts;

namespace RetailPOS.Persistence.DesignTime
{
    public class RetailPosDesignTimeDbContextFactory : IDesignTimeDbContextFactory<RetailPosDbContext>
    {
        public RetailPosDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<RetailPosDbContext>();
            optionsBuilder.UseSqlite("Data Source=retailpos.db");

            return new RetailPosDbContext(optionsBuilder.Options);
        }
    }
}
