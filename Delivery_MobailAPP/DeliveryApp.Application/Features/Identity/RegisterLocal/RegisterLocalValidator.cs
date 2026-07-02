namespace DeliveryApp.Application.Features.Identity.RegisterLocal
{
    public static class RegisterLocalValidator
    {
        public static RegisterLocalValidatedInput Validate(RegisterLocalRequest request, DateOnly today)
        {
            if (request is null) 
                throw new Exception("Request is required.");

            if (today == default) 
                throw new Exception("Invalid system date.");

            var fullName = IdentityInputValidator.ValidateFullName(request.FullName);

            var phone = IdentityInputValidator.ValidatePhone(request.Phone);

            var birthDate = IdentityInputValidator.ValidateBirthDate(request.BirthDate);

            var password = IdentityInputValidator.ValidatePassword(request.Password);

            var photoUrl = IdentityInputValidator.ValidatePhotoUrl(request.PhotoUrl);

            return new RegisterLocalValidatedInput
            (
                fullName,
                phone,
                birthDate,
                password,
                photoUrl
            );
        }
    }

    public sealed record RegisterLocalValidatedInput
    (
        string FullName,
        string Phone,
        DateOnly BirthDate,
        string Password,
        string? PhotoUrl
    );
}