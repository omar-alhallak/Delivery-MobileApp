namespace DeliveryApp.Application.Features.Identity.RegisterLocal
{
    public sealed class RegisterLocalRequest // DTO الي ستلمناه
    {
        public string FullName { get; init; } = null!;
        public string Phone { get; init; } = null!;
        public string BirthDate { get; init; } = null!;
        public string Password { get; init; } = null!;
        public string? PhotoUrl { get; init; }
    }
}