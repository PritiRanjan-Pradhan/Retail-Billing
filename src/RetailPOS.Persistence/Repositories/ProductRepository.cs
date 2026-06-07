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
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(RetailPosDbContext context)
            : base(context)
        {
        }

        public async Task<bool> SkuExistsAsync(string sku, Guid? excludeId = null)
        {
            var query = _dbSet.AsNoTracking().Where(p => p.SKU == sku);

            if (excludeId.HasValue)
            {
                query = query.Where(p => p.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<Product?> GetBySkuAsync(string sku)
        {
            return await _dbSet.AsNoTracking().FirstOrDefaultAsync(p => p.SKU == sku);
        }

        public async Task<Product?> GetByBarcodeAsync(string barcode)
        {
            return await _dbSet.AsNoTracking().FirstOrDefaultAsync(p => p.Barcode == barcode);
        }

        public async Task<IEnumerable<Product>> SearchAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return await GetAllAsync();
            }

            query = query.Trim().ToLowerInvariant();

            return await _dbSet.AsNoTracking()
                .Where(p => p.Name.ToLower().Contains(query)
                            || p.SKU.ToLower().Contains(query)
                            || (p.Barcode != null && p.Barcode.ToLower().Contains(query)))
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetLowStockAsync()
        {
            return await _dbSet.AsNoTracking()
                .Where(p => p.StockQuantity <= p.ReorderLevel)
                .OrderBy(p => p.StockQuantity)
                .ToListAsync();
        }
    }
}
