using DeliveryApp.Domain.Enums.CustomerEnums;

namespace DeliveryApp.Application.Features.Addresses
{
    public class UpdateAddressRequest
    {
        public string? Label { get; init; } = null!;
        public AddressType AddressType { get; init; } 
        public string? BuildingName { get; init; } = null!;
        public string? Floor { get; init; } = null!;
        public string? DoorInfo { get; init; } = null!;
        public string? Notes { get; init; } = null!;
    }
}