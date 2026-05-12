namespace DeliveryApp.Application.Features.Identity.UpdateMyProfile
{
    public sealed record UpdateMyProfileResponse
    {
        public Guid UserId { get; init; }
        public string? FullName { get; init; }
        public string? Phone { get; init; }
        public string? PhotoUrl { get; init; }
        public DateOnly? BirthDate { get; init; }
        public bool IsProfileComplete { get; init; }
    }
}