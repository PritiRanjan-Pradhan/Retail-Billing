using System.Threading.Tasks;
using RetailPOS.Application.Interfaces;
using RetailPOS.Persistence.Contexts;

namespace RetailPOS.Persistence.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly RetailPosDbContext _context;

        public UnitOfWork(RetailPosDbContext context)
        {
            _context = context;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
