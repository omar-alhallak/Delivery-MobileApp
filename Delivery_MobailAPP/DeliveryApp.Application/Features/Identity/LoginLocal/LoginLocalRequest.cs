using DeliveryApp.Domain.Enums.IdentityEnums;

namespace DeliveryApp.Application.Features.Identity.LoginLocal
{
    public sealed record LoginLocalRequest
    {
        public string Phone { get; init; } = null!;
        public string Password { get; init; } = null!;
        public string DeviceID { get; init; } = null!;
        public ClientType ClientType { get; init; }
    }
}