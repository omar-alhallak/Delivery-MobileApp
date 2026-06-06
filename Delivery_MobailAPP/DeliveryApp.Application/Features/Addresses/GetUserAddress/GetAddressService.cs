using DeliveryApp.Application.Interfaces.Addresses;
using DeliveryApp.Domain.Entities.Customers;

namespace DeliveryApp.Application.Features.Addresses.GetUserAddress
{
    public class GetAddressService
    {
        private readonly IAddressRepository _repository;

        public GetAddressService(IAddressRepository repository)
        {
            _repository = repository;
        }

        public async Task<IReadOnlyList<GetAddressResponse>> GetListAsync(GetUserAddressRequest request)
        {
            var entities = await _repository.GetByUserIdAsync(request.UserID);
            return entities
                .Select(x => MapToResponse(x))
                .ToList();
        }
        public async Task<GetAddressResponse?> GetByIdAsync(AddressID addressId)
        {
            var entity = await _repository.GetByIdAsync(addressId);

            if (entity is null)
                return null;

            return MapToResponse(entity);
        }
        public async Task SetAsDefaultAsync(UserID userId, AddressID addressId, CancellationToken ct = default)
        {
            await _repository.SetAsDefaultAsync(userId, addressId, ct);
        }
        private static GetAddressResponse MapToResponse(Address entity)
        {
            return new GetAddressResponse
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
