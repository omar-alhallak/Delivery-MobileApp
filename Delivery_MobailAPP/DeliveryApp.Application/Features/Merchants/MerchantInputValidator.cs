using System.Text.RegularExpressions;

namespace DeliveryApp.Application.Features.Merchants
{
    public static partial class MerchantInputValidator
    {
        // -------------------------
        //       Required Text
        // -------------------------

        // ------ Validation ------
        // 1_ تأكد أن الحقل ليس فارغاً
        // 2_ تنظيف المسافات
        // 3_ إرجاع النص جاهز للاستخدام

        public static string ValidateRequiredText(string value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new Exception($"{fieldName} is required.");

            value = NormalizeSpaces(value);

            if (string.IsNullOrWhiteSpace(value))
                throw new Exception($"{fieldName} is required.");

            return value;
        }

        // -------------------------
        //       Description
        // -------------------------

        // ------ Validation ------
        // 1_ إذا الحقل فارغ يرجع null
        // 2_ تنظيف المسافات
        // 3_ إرجاع النص جاهز للاستخدام

        public static string? ValidateOptionalText(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;

            return NormalizeSpaces(value);
        }

        // -------------------------
        //          Photo
        // -------------------------

        // ------ Validation ------
        // 1_ إذا الحقل فارغ يرجع null
        // 2_ تنظيف المسافات
        // 3_ قبول فقط:
        //  A_ http
        //  B_ https

        public static string? ValidateOptionalUrl(string? value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;

            value = value.Trim();

            if (!Uri.TryCreate(value, UriKind.Absolute, out var uri) || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)) 
                throw new Exception($"Invalid {fieldName} URL.");

            return value;
        }

        public static TimeSpan ToPreparationTime(int minutes)
        {
            return TimeSpan.FromMinutes(minutes);
        }

        // -------------------------
        //          Helpers
        // -------------------------

        private static string NormalizeSpaces(string value) => SpaceRegex().Replace(value.Trim(), " ");

        // -------------------------
        //          Regex
        // -------------------------

        [GeneratedRegex(@"\s+")]
        private static partial Regex SpaceRegex();
    }
}