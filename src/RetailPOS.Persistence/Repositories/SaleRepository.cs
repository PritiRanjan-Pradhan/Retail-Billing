using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RetailPOS.Application.Interfaces;
using RetailPOS.Domain.Entities;
using RetailPOS.Persistence.Contexts;

namespace RetailPOS.Persistence.Repositories
{
    public class SaleRepository : GenericRepository<Sale>, ISaleRepository
    {
        public SaleRepository(RetailPosDbContext context)
            : base(context)
        {
        }

        public async Task<IEnumerable<Sale>> GetRecentSalesAsync(int count = 20)
        {
            return await _dbSet.AsNoTracking()
                .OrderByDescending(s => s.Date)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Sale>> GetSalesByDateRangeAsync(DateTime start, DateTime end)
        {
            return await _dbSet.AsNoTracking()
                .Where(s => s.Date >= start && s.Date <= end)
                .OrderByDescending(s => s.Date)
                .ToListAsync();
        }
    }
}
