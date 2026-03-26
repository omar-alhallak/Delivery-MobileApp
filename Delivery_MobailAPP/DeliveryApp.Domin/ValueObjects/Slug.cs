using System.Text;
using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.DomainExceptions;

namespace DeliveryApp.Domain.ValueObjects
{
    public sealed class Slug // نص رابط
    {
        public string Value { get; } // القيمة بعد التنظيف

        private Slug(string value) => Value = value; // لمنع إنشاء الصف إلا عن طريق Create

        public static Slug Create(string? input) // تتحقق و تنظف و تنشأ Slug
        {
            if (string.IsNullOrWhiteSpace(input)) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(input));

            var normalized = Normalize(input);

            if (normalized.Length > 80) throw new DomainValidationException
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, nameof(input));

            return new Slug(normalized);
        }

        private static string Normalize(string value) // ينظف النص ويجعله ك رابط
        {
            value = value.Trim().ToLowerInvariant();

            var builder = new StringBuilder();
            bool lastDash = false;

            foreach (var ch in value)
            {
                if (char.IsLetterOrDigit(ch))
                {
                    builder.Append(ch);
                    lastDash = false;
                }
                else if (ch == ' ' || ch == '-' || ch == '_')
                {
                    if (!lastDash && builder.Length > 0)
                    {
                        builder.Append('-');
                        lastDash = true;
                    }
                }
            }

            var result = builder.ToString().Trim('-');

            if (string.IsNullOrWhiteSpace(result)) throw new DomainValidationException
                    (ValidationErrors.InvalidSlugCode, ValidationErrors.InvalidSlugMessage, nameof(value));

            return result;
        }

        public override string ToString() => Value; // لتحويل الكلاس لنص

        // لمقارنة القيم المدخلة من دونه لا يمكن المقارنة لان كل قيمة تعتبر مختلفة بذاكرة
        public override bool Equals(object? obj) => obj is Slug other && string.Equals(Value, other.Value, StringComparison.Ordinal);

        // تعطي رقم يمثل القيمة للمقارنة
        public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value);
    }
}