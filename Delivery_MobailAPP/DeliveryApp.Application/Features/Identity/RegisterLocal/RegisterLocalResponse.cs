namespace DeliveryApp.Application.Features.Identity.RegisterLocal
{
    public sealed class RegisterLocalResponse // DTO المرسل
    {
        public Guid UserId { get; init; }
        public string PublicId { get; init; } = null!;
    }
}