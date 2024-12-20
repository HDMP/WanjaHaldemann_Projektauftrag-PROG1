using SwissAddressManager.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SwissAddressManager.Services.Interfaces
{
    public interface IAddressService
    {
        Task<IEnumerable<Address>> GetAllAddressesAsync();
        Task AddAddressAsync(Address address);
    }
}
