namespace DeliveryApp.Application.Features.Identity.RefreshToken
{
    public sealed record RefreshTokenResponse
    {
        public string AccessToken { get; init; } = null!;
        public string RefreshToken { get; init; } = null!;
    }
}