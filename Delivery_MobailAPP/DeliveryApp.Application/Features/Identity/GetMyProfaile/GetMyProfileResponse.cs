namespace DeliveryApp.Application.Features.Identity.GetMyProfile
{
    public sealed record GetMyProfileResponse
    {
        public Guid UserId { get; init; }

        public string? PublicId { get; init; }

        public string? Phone { get; init; }

        public string? Email { get; init; }

        public string? FullName { get; init; }

        public string? PhotoUrl { get; init; }

        public DateOnly? BirthDate { get; init; }

        public bool IsProfileComplete { get; init; }

        public decimal CustomerAverageRating { get; init; }

        public int CustomerRatingsCount { get; init; }
    }
}