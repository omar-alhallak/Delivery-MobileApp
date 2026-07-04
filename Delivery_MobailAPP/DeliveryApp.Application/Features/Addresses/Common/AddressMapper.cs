using DeliveryApp.Domain.Entities.Customers;

namespace DeliveryApp.Application.Features.Addresses.Common
{
    public static class AddressMapper // يحول Address من الدومين إلى DTO
    {
        public static AddressDto ToDto(Address address)
        {
            return new AddressDto
            {
                Id = address.ID.Value,
                UserId = address.UserID.Value,
                Label = address.Label,
                AddressType = address.AddressType,
                Latitude = address.Location.Latitude,
                Longitude = address.Location.Longitude,
                BuildingName = address.BuildingName,
                Floor = address.Floor,
                DoorInfo = address.DoorInfo,
                Notes = address.Notes,
                IsDefault = address.IsDefault,
                IsTemporary = address.IsTemporary,
                IsActive = address.IsActive,
                CreatedAt = address.CreatedAt
            };
        }
    }
}