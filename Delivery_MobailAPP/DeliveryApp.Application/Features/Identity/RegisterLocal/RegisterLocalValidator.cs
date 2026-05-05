using System.Text.RegularExpressions;

namespace DeliveryApp.Application.Features.Identity.RegisterLocal
{
    public static partial class RegisterLocalValidator // Validation الخاص بخدمة إنشاء الحساب
    {
        private const int MinPasswordLength = 8;
        private const int MaxPasswordLength = 64;

        public static RegisterLocalValidatedInput Validate(RegisterLocalRequest request, DateOnly today) // فحص جميع الحقول لإرسال نسخة منظفة وصالحة للاستخدام
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

        //         Validation
        // 1_ تأكد أن الحقل ليس فارغاً
        // 2_ تنظيف المسافات
        // 3_ الأسم لا يبدأ برقم أو رمز جصراً حرف
        // 4_ منع تكرارالشرطات
        // 5_ فلترى الشرطات
        // 6_ الأسم يحوي فقط على:
        //  A_ أحرف عربية
        //  B_ أحرف أجنبية
        //  C_ أرقام
        //  D_ فراغات
        //  E_ الشرطات(_ -)د

        private static string ValidateFullName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new Exception("Full name is required.");

            value = NormalizeSpaces(value);
            value = NormalizeNameSymbols(value);

            if (string.IsNullOrWhiteSpace(value))
                throw new Exception("Full name is required.");

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

        //         Validation
        // 1_ تأكد أن الحقل ليس فارغاً
        // 2_ تنظيف المسافات
        // 3_ تأكد من أن الرقم بشكل D +963 9XXXXXXXX

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

        //         Validation
        // 1_ تأكد أن الحقل ليس فارغاً
        // 2_ لازم حصراً يكون بصيغة yyyy/mm/dd
        // 3_ تأكد من صحة تاريخ

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
        //         Password
        // -------------------------

        //         Validation
        // 1_ تأكد أن الحقل ليس فارغاً
        // 2_ أقل شي 8 محارف و أكثر شي 64
        // 3_ تحوي على حرف واحد على الأقل من أحرف أجنبية كبيرة
        // 4_ تحوي على حرف واحد على الأقل من أحرف أجنبية صغيرة
        // 5_ تحوي على رقم واحد على الأقل 
        // 6_ تحوي على رمز واحد على الأقل من الرموز(! @ # $ % ^ & *)د
        // 7_ تأكد من أن كلمة السر تحوي فقط على:
        //  A_ أحرف أجنبية  كبيرة
        //  B_ أحرف أجنبية صغيرة
        //  C_ الرموز(! @ # $ % ^ & *)د
        //  D_ أرقام

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

        //         Validation
        // 1_ تنظيف المسافات
        // 3_ تأكد من صحة الرابط من خلال قبول فقط:
        //  A_ http
        //  B_ https

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

        private static string NormalizeSpaces(string value) => SpaceRegex().Replace(value.Trim(), " "); // تنظيف المسافات

        private static string NormalizeNameSymbols(string value) // فلترى الشرطات
        {
            value = NameSymbolSpacesRegex().Replace(value, "$1");
            value = value.Trim(' ', '-', '_');

            return value;
        }

        private static bool IsSpecialCharacter(char c) => "!@#$%^&*".Contains(c); // فحص إذا رمز أحد الرموز(! @ # $ % ^ & *)د

        // -------------------------
        //          Regex
        // -------------------------

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

        [GeneratedRegex(@"\s*([_-])\s*")]
        private static partial Regex NameSymbolSpacesRegex();

        [GeneratedRegex(@"\s+")]
        private static partial Regex SpaceRegex();
    }

    // نسخة المنقحة
    public sealed record RegisterLocalValidatedInput(string FullName, string Phone, DateOnly BirthDate, string Password, string? PhotoUrl);
}