namespace DeliveryApp.Application.Features.Identity.LoginLocal
{
    public sealed record LoginLocalResponse
    {
        public Guid UserId { get; init; }
        public string PublicId { get; init; } = null!;
        public string AccessToken { get; init; } = null!;
        public string RefreshToken { get; init; } = null!;
        public bool IsProfileComplete { get; init; }
    }
}