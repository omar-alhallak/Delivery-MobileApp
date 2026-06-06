using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeliveryApp.Application.Interfaces.Addresses;
using DeliveryApp.Domain.Entities.Customers;

namespace DeliveryApp.Application.Features.Addresses.UpdateAddress
{
    public class UpdateAddressService
    {
        private readonly IAddressRepository _repository;

        public UpdateAddressService(IAddressRepository repository)
        {
            _repository = repository;
        }
        public async Task<UpdateAddressResponse> UpdateAsync(UserID userId, AddressID addressId, UpdateAddressRequest request, CancellationToken ct = default)
        {
            var address = await _repository.GetByIdAsync(addressId, ct);

            if (address is null || address.UserID != userId)
                throw new KeyNotFoundException("Address not found or unauthorized.");

            address.UpdateDetails(
                request.Label,
                request.AddressType,
                request.BuildingName,
                request.Floor,
                request.DoorInfo,
                request.Notes
            );

            await _repository.UpdateAsync(address, ct);

            return MapToResponse(address);
        }
        private static UpdateAddressResponse MapToResponse(Address entity)
        {
            return new UpdateAddressResponse
            {
                ID = entity.ID,
                UserID = entity.UserID,
                Label = entity.Label,
                AddressType = entity.AddressType,
                Location = entity.Location,
                BuildingName = entity.BuildingName,
                Floor = entity.Floor,
                DoorInfo = entity.DoorInfo,
                Notes = entity.Notes,
                IsDefault = entity.IsDefault,
                IsTemporary = entity.IsTemporary,
                IsActive = entity.IsActive,
                CreatedAt = entity.CreatedAt
            };
        }
    }
}
