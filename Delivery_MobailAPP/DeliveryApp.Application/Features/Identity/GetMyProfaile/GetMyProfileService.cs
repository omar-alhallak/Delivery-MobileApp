using DeliveryApp.Application.Interfaces.IdentityRepositoresInterfaces;

namespace DeliveryApp.Application.Features.Identity.GetMyProfile
{
    public sealed class GetMyProfileService
    {
        private readonly IGetMyProfileRepository _repository;

        public GetMyProfileService(IGetMyProfileRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<GetMyProfileResponse> ExecuteAsync(Guid currentUserId, CancellationToken ct = default)
        {
            var userID = UserID.From(currentUserId);

            var user = await _repository.GetUserByIdAsync(userID, ct);

            if (user is null)
                throw new Exception("User not found.");

            return new GetMyProfileResponse
            {
                UserId = user.ID.Value,

                PublicId = user.PublicID?.Value,

                Phone = user.Phone,

                Email = user.Email,

                FullName = user.FullName,

                PhotoUrl = user.PhotoUrl,

                BirthDate = user.BirthDate,

                IsProfileComplete = user.IsProfileComplete,

                CustomerAverageRating = user.CustomerAverageRating,

                CustomerRatingsCount = user.CustomerRatingsCount
            };
        }
    }
}