namespace DeliveryApp.Application.Features.Identity.UpdateMyProfile
{
    public static class UpdateMyProfileValidator 
    {
        public static UpdateMyProfileValidatedInput Validate(UpdateMyProfileRequest request, DateOnly today)
        {
            if (request is null)
                throw new Exception("Request is required.");

            if (today == default)
                throw new Exception("Invalid system date.");

            var fullName = request.FullName is null ? null : IdentityInputValidator.ValidateFullName(request.FullName);

            var birthDate = request.BirthDate is null ? (DateOnly?)null : IdentityInputValidator.ValidateBirthDate(request.BirthDate);

            var photoUrl = IdentityInputValidator.ValidatePhotoUrl(request.PhotoUrl);

            var phone = request.Phone is null ? null : IdentityInputValidator.ValidatePhone(request.Phone);

            var newPassword = request.NewPassword is null ? null : IdentityInputValidator.ValidatePassword(request.NewPassword);

            var currentPassword = request.CurrentPassword;

            if ((phone is not null || newPassword is not null) && string.IsNullOrWhiteSpace(currentPassword)) 
            {
                throw new Exception("Current password is required to change phone or password.");
            }

            return new UpdateMyProfileValidatedInput
            (
                fullName,
                birthDate,
                photoUrl,
                phone,
                currentPassword,
                newPassword
            );
        }
    }

    public sealed record UpdateMyProfileValidatedInput
    (
        string? FullName,
        DateOnly? BirthDate,
        string? PhotoUrl,
        string? Phone,
        string? CurrentPassword,
        string? NewPassword
    );
}