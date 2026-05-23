using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeliveryApp.Domain.Enums.CustomerEnums;
using DeliveryApp.Domain.ValueObjects;

namespace DeliveryApp.Application.Features.Addresses
{
    public sealed class CreateUserAddressRequest
    {
        public UserID UserID { get; init; } 
        public string? Label { get; init; } = null!;
        public AddressType? AddressType { get; init; } = null!;
        public GeoPoint Location { get; init; } = null!;    
        public string? BuildingName { get; init; } = null!;
        public string? Floor { get; init; } = null!;
        public string? DoorInfo { get; init; } = null!;
        public string? Notes { get; init; } = null!;
        public bool IsDefault { get; init; }
        public bool IsTemporary { get; init; }
    }
}
