using DeliveryApp.Application.Interfaces.Services;
using DeliveryApp.Application.Interfaces.IdentityRepositoresInterfaces;

namespace DeliveryApp.Application.Features.Identity.UpdateMyProfile
{
    public sealed class UpdateMyProfileService
    {
        private readonly IUpdateMyProfileRepository _repository;
        private readonly IPasswordHasher _passwordHasher;

        public UpdateMyProfileService(IUpdateMyProfileRepository repository, IPasswordHasher passwordHasher)
        {
            _repository = repository;
            _passwordHasher = passwordHasher;
        }

        public async Task<UpdateMyProfileResponse> ExecuteAsync(Guid userId, UpdateMyProfileRequest request, CancellationToken ct = default)
        {
            var userID = UserID.From(userId);
            var now = DateTimeOffset.UtcNow;
            var today = DateOnly.FromDateTime(now.UtcDateTime);

            // -------------------------
            //        Validation
            // -------------------------

            var input = UpdateMyProfileValidator.Validate(request, today);

            // -------------------------
            //          Get User
            // -------------------------

            var user = await _repository.GetUserByIdAsync(userID, ct);

            if (user is null)
                throw new Exception("User not found.");

            // -------------------------
            //      Sensitive Changes
            // -------------------------

            var wantsSensitiveChange = input.Phone is not null || input.NewPassword is not null;

            if (wantsSensitiveChange)
            {
                var identity = await _repository.GetLocalIdentityAsync(user.ID, ct);

                if (identity is null)
                    throw new Exception("Local identity not found.");

                var isValidPassword = _passwordHasher.Verify(input.CurrentPassword!, identity.PasswordHash!);

                if (!isValidPassword)
                    throw new Exception("Current password is incorrect.");

                if (input.Phone is not null && input.Phone != user.Phone)
                {
                    var phoneExists = await _repository.PhoneExistsAsync
                    (
                        input.Phone,
                        user.ID,
                        ct
                    );

                    if (phoneExists)
                        throw new Exception("Phone already exists.");
                }

                if (input.NewPassword is not null)
                {
                    var newPasswordHash = _passwordHasher.Hash(input.NewPassword);

                    identity.ChangeLocalPasswordHash(newPasswordHash);
                }
            }

            // -------------------------
            //       Update Profile
            // -------------------------

            user.UpdateProfile
            (
                email: user.Email,
                phone: input.Phone ?? user.Phone,
                fullName: input.FullName ?? user.FullName,
                photoUrl: input.PhotoUrl ?? user.PhotoUrl
            );

            if (input.BirthDate.HasValue) user.ChangeBirthDate(input.BirthDate.Value, today);

            if (!user.IsProfileComplete) user.MarkProfileAsComplete();

            // -------------------------
            //            Save
            // -------------------------

            await _repository.SaveChangesAsync(ct);

            // -------------------------
            //          Response
            // -------------------------

            return new UpdateMyProfileResponse
            {
                UserId = user.ID.Value,
                FullName = user.FullName,
                Phone = user.Phone,
                PhotoUrl = user.PhotoUrl,
                BirthDate = user.BirthDate,
                IsProfileComplete = user.IsProfileComplete
            };
        }
    }
}