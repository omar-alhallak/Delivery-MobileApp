namespace DeliveryApp.Application.Features.Identity.RegisterLocal
{
    public sealed class RegisterLocalResponse
    {
        public Guid UserId { get; init; }
        public string PublicId { get; init; } = null!;
    }
}