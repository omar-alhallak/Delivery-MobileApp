namespace DeliveryApp.Infrastructure.Options
{
    public sealed class JwtOptions
    {
        public string SecretKey { get; init; } = null!;
        public int AccessTokenMinutes { get; init; }
    }
}