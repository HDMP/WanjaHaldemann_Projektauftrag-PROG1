using System.Collections.Generic;
using System.Threading.Tasks;
using SwissAddressManager.Data.Models;
using SwissAddressManager.Data.Repositories;
using SwissAddressManager.Services.Interfaces;

namespace SwissAddressManager.Services.Implementations
{
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _addressRepository;

        public AddressService(IAddressRepository addressRepository)
        {
            _addressRepository = addressRepository;
        }

        public async Task<IEnumerable<Address>> GetAllAddressesAsync()
        {
            return await _addressRepository.GetAllAsync();
        }

        public async Task AddAddressAsync(Address address)
        {
            await _addressRepository.AddAsync(address);
        }
    }
}
