using DeliveryApp.Domain.Enums.IdentityEnums;

namespace DeliveryApp.Application.Features.Identity.Logout
{
    public sealed record LogoutRequest
    {
        public string DeviceID { get; init; } = null!;
        public ClientType ClientType { get; init; }
    }
}