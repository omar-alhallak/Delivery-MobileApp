using System.Text.RegularExpressions;
using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.DomainExceptions;

namespace DeliveryApp.Domain.ValueObjects
{
    public readonly record struct PublicCode // الكود الذي يظهر للمستخدم مثال ORD-000000
    {
        private static readonly Regex FullFormatRegex = new(@"^[A-Z]{1,5}-\d{6,}$", RegexOptions.Compiled); // للتحقق من الكود كاملاً
        private static readonly Regex PrefixFormatRegex = new(@"^[A-Z]{1,5}$", RegexOptions.Compiled); // تحقق من البداية مثال ORD

        public string Value { get; } // القيمة النهائية بعد التحقق

        private PublicCode(string value) => Value = value; // لمنع إنشاء الصف إلا عن طريق Create


        public static PublicCode From(string value) // للتحقق من الكود أنه صحيح
        {
            if (string.IsNullOrWhiteSpace(value)) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(value));

            value = value.Trim().ToUpperInvariant();

            if (!FullFormatRegex.IsMatch(value)) throw new DomainValidationException
                    (ValidationErrors.InvalidFormatCode, ValidationErrors.InvalidFormatMessage, nameof(value));

            return new PublicCode(value);
        }

        public static PublicCode Create(string prefix, long number) // يستخدم مع سيكونسي لينشئ النظام الكود
        {
            if (string.IsNullOrWhiteSpace(prefix)) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(prefix));

            prefix = prefix.Trim().ToUpperInvariant();

            if (!PrefixFormatRegex.IsMatch(prefix)) throw new DomainValidationException
                    (ValidationErrors.InvalidFormatCode, ValidationErrors.InvalidFormatMessage, nameof(prefix));

            if (number <= 0) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(number));

            var formatted = $"{prefix}-{number.ToString().PadLeft(6, '0')}";
            return new PublicCode(formatted);
        }

        public override string ToString() => Value; // لتحويل الكلاس لنص
    }
}