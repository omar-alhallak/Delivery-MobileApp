using DeliveryApp.Domain.Enums.IdentityEnums;

namespace DeliveryApp.Application.Features.Identity.RefreshToken
{
    public sealed record RefreshTokenRequest
    {
        public Guid UserId { get; init; }
        public string RefreshToken { get; init; } = null!;
        public string DeviceID { get; init; } = null!;
        public ClientType ClientType { get; init; }
    }
}