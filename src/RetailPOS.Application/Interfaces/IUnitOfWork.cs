using System.Threading.Tasks;

namespace RetailPOS.Application.Interfaces
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync();
    }
}
