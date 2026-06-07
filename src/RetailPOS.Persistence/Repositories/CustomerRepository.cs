using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RetailPOS.Application.Interfaces;
using RetailPOS.Domain.Entities;
using RetailPOS.Persistence.Contexts;

namespace RetailPOS.Persistence.Repositories
{
    public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(RetailPosDbContext context)
            : base(context)
        {
        }

        public async Task<Customer?> GetByMobileAsync(string mobile)
        {
            return await _dbSet.AsNoTracking().FirstOrDefaultAsync(c => c.Mobile == mobile);
        }

        public async Task<IEnumerable<Customer>> SearchAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return await GetAllAsync();
            }

            query = query.Trim().ToLowerInvariant();

            return await _dbSet.AsNoTracking()
                .Where(c => c.Name.ToLower().Contains(query)
                            || c.Mobile.Contains(query))
                .ToListAsync();
        }
    }
}
