using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RetailPOS.Application.Interfaces;
using RetailPOS.Persistence.Contexts;

namespace RetailPOS.Persistence.Repositories
{
    public class GenericRepository<T> : IRepository<T> where T : class
    {
        protected readonly RetailPosDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(RetailPosDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }
    }
}
