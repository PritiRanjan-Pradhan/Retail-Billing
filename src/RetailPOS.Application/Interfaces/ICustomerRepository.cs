using System.Collections.Generic;
using System.Threading.Tasks;
using RetailPOS.Domain.Entities;

namespace RetailPOS.Application.Interfaces
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        Task<Customer?> GetByMobileAsync(string mobile);
        Task<IEnumerable<Customer>> SearchAsync(string query);
    }
}
