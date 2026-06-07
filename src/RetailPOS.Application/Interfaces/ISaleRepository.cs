using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RetailPOS.Domain.Entities;

namespace RetailPOS.Application.Interfaces
{
    public interface ISaleRepository : IRepository<Sale>
    {
        Task<IEnumerable<Sale>> GetSalesByDateRangeAsync(DateTime start, DateTime end);
        Task<IEnumerable<Sale>> GetRecentSalesAsync(int count = 20);
    }
}
