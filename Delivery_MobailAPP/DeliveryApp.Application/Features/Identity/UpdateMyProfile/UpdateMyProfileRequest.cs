namespace DeliveryApp.Application.Features.Identity.UpdateMyProfile
{
    public sealed record UpdateMyProfileRequest
    {
        public string? FullName { get; init; }
        public string? BirthDate { get; init; }
        public string? PhotoUrl { get; init; }

        public string? Phone { get; init; }
        public string? CurrentPassword { get; init; }
        public string? NewPassword { get; init; }
    }
}