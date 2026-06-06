using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeliveryApp.Application.Interfaces.Addresses;
using DeliveryApp.Domain.Entities.Customers;

namespace DeliveryApp.Application.Features.Addresses.CreateAddress
{
    public class CreateAddressService
    {
        private readonly IAddressRepository _repository;

        public CreateAddressService(IAddressRepository repository)
        {
            _repository = repository;
        }
        public async Task<CreateAddressResponse> CreateAsync(UserID userId, CreateUserAddressRequest request)
        {
            var now = DateTimeOffset.UtcNow;

            var addressEntity = new Address(
                AddressID.New(),
                userId,
                request.Location.Latitude,
                request.Location.Longitude,
                now
            );

            addressEntity.Complete(
                request.Label!,
                request.AddressType!.Value,
                request.BuildingName!,
                request.Floor!,
                request.DoorInfo!,
                request.Notes
            );

            if (request.IsDefault)
            {
                addressEntity.SetAsDefault();
            }

            var createdEntity = await _repository.CreateAsync(userId, addressEntity);

            return MapToResponse(createdEntity);
        }
        private static CreateAddressResponse MapToResponse(Address entity)
        {
            return new CreateAddressResponse
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
