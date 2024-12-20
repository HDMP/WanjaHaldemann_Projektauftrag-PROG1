using SwissAddressManager.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SwissAddressManager.Data.Repositories
{
    public interface IAddressRepository
    {
        Task<IEnumerable<Address>> GetAllAsync();
        Task<Address> GetByIdAsync(int id);
        Task AddAsync(Address address);
        Task UpdateAsync(Address address);
        Task DeleteAsync(int id);
    }
}
