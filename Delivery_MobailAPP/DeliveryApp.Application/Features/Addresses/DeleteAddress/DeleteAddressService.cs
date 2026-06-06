using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeliveryApp.Application.Interfaces.Addresses;

namespace DeliveryApp.Application.Features.Addresses.DeleteAddress
{
    public class DeleteAddressService
    {
        private readonly IAddressRepository _repository;

        public DeleteAddressService(IAddressRepository repository)
        {
            _repository = repository;
        }
        public async Task DeleteAsync(UserID userId, AddressID addressId, CancellationToken ct = default)
        {
            // 1. جلب العنوان للتحقق
            var address = await _repository.GetByIdAsync(addressId, ct);

            // 2. التحقق من وجوده وملكيته
            if (address is null || address.UserID != userId)
                throw new KeyNotFoundException("Address not found or unauthorized.");

            // 3. الحذف
            await _repository.DeleteAsync(address, ct);
        }
    }
}
