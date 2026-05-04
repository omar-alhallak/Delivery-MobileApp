using System.Text.RegularExpressions;

namespace DeliveryApp.Application.Features.Identity.RegisterLocal
{
    public static partial class RegisterLocalValidator
    {
        private const int MinPasswordLength = 8;
        private const int MaxPasswordLength = 64;

        public static RegisterLocalValidatedInput Validate(RegisterLocalRequest request, DateOnly today)
        {
            if (request is null) throw new Exception("Request is required.");
            if (today == default) throw new Exception("Invalid system date.");

            var fullName = ValidateFullName(request.FullName);
            var phone = ValidatePhone(request.Phone);
            var birthDate = ParseBirthDate(request.BirthDate);
            var password = ValidatePassword(request.Password);
            var photoUrl = ValidatePhotoUrl(request.PhotoUrl);

            return new RegisterLocalValidatedInput(fullName, phone, birthDate, password, photoUrl);
        }

        // -------------------------
        //          FullName
        // -------------------------

        private static string ValidateFullName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new Exception("Full name is required.");

            value = NormalizeSpaces(value);

            if (!FullNameRegex().IsMatch(value))
                throw new Exception("Full name contains invalid characters.");

            if (char.IsDigit(value[0]) || value[0] == '_' || value[0] == '-')
                throw new Exception("Full name cannot start with a number, underscore, or dash.");

            if (value.All(c => char.IsDigit(c) || c == ' ' || c == '_' || c == '-'))
                throw new Exception("Full name cannot be numbers or symbols only.");

            if (RepeatedNameSymbolsRegex().IsMatch(value))
                throw new Exception("Full name cannot contain repeated underscores or dashes.");

            return value;
        }

        // -------------------------
        //           Phone
        // -------------------------

        private static string ValidatePhone(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new Exception("Phone is required.");

            value = NormalizeSpaces(value);

            if (!PhoneRegex().IsMatch(value))
                throw new Exception("Invalid phone format.");

            return value;
        }

        // -------------------------
        //         BirthDate
        // -------------------------

        private static DateOnly ParseBirthDate(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new Exception("Birth date is required.");

            value = value.Trim();

            if (!DateRegex().IsMatch(value))
                throw new Exception("Birth date must be yyyy/MM/dd.");

            if (!DateOnly.TryParseExact(value, "yyyy/MM/dd", out var birthDate))
                throw new Exception("Invalid birth date.");

            return birthDate;
        }

        // -------------------------
        // Password
        // -------------------------

        private static string ValidatePassword(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new Exception("Password is required.");

            if (value.Length < MinPasswordLength)
                throw new Exception("Password too short.");

            if (value.Length > MaxPasswordLength)
                throw new Exception("Password too long.");

            if (!PasswordAllowedCharactersRegex().IsMatch(value))
                throw new Exception("Password can contain English letters, numbers, and special characters only.");

            if (!value.Any(c => c is >= 'A' and <= 'Z'))
                throw new Exception("Password must contain at least one English uppercase letter.");

            if (!value.Any(c => c is >= 'a' and <= 'z'))
                throw new Exception("Password must contain at least one English lowercase letter.");

            if (!value.Any(char.IsDigit))
                throw new Exception("Password must contain at least one number.");

            if (!value.Any(IsSpecialCharacter))
                throw new Exception("Password must contain at least one special character.");

            return value;
        }

        // -------------------------
        //          Photo
        // -------------------------

        private static string? ValidatePhotoUrl(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;

            value = value.Trim();

            if (!Uri.TryCreate(value, UriKind.Absolute, out var uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
                throw new Exception("Invalid photo URL.");

            return value;
        }

        // -------------------------
        //          Helpers
        // -------------------------

        private static string NormalizeSpaces(string value) => SpaceRegex().Replace(value.Trim(), " ");

        private static bool IsSpecialCharacter(char c) => "!@#$%^&*".Contains(c);

        [GeneratedRegex(@"^[\p{IsArabic}A-Za-z0-9 _-]+$")]
        private static partial Regex FullNameRegex();

        [GeneratedRegex(@"[_-]{2,}")]
        private static partial Regex RepeatedNameSymbolsRegex();

        [GeneratedRegex(@"^\+963 9\d{8}$")]
        private static partial Regex PhoneRegex();

        [GeneratedRegex(@"^\d{4}/\d{2}/\d{2}$")]
        private static partial Regex DateRegex();

        [GeneratedRegex(@"^[A-Za-z0-9!@#$%^&*]+$")]
        private static partial Regex PasswordAllowedCharactersRegex();

        [GeneratedRegex(@"\s+")]
        private static partial Regex SpaceRegex();
    }
    
    public sealed record RegisterLocalValidatedInput( string FullName, string Phone, DateOnly BirthDate, string Password, string? PhotoUrl);
}