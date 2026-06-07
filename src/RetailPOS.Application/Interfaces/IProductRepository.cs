using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RetailPOS.Domain.Entities;

namespace RetailPOS.Application.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<bool> SkuExistsAsync(string sku, Guid? excludeId = null);
        Task<Product?> GetBySkuAsync(string sku);
        Task<Product?> GetByBarcodeAsync(string barcode);
        Task<IEnumerable<Product>> SearchAsync(string query);
        Task<IEnumerable<Product>> GetLowStockAsync();
    }
}
