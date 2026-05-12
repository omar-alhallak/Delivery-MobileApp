namespace DeliveryApp.Application.Features.Identity.RegisterLocal
{
    public sealed record RegisterLocalResponse // DTO المرسل
    {
        public Guid UserId { get; init; }
        public string PublicId { get; init; } = null!;
    }
}