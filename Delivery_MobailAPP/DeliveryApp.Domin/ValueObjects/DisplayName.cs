using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.DomainExceptions;

namespace DeliveryApp.Domain.ValueObjects
{
    public sealed class DisplayName // فرض قواعد على أسماء الموجودة ب Catalog
    {
        public string Value { get; } // القيمة بعد التنظيف

        private DisplayName(string value) => Value = value; // لمنع إنشاء الصف إلا عن طريق Create

        public static DisplayName Create(string? input, int maxLength, string fieldName) // تتحقق و تنظف و تنشأ الأسم
        {
            if (string.IsNullOrWhiteSpace(fieldName)) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(fieldName));   

            if (maxLength <= 0) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(maxLength));

            if (string.IsNullOrWhiteSpace(input)) throw new DomainValidationException 
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, fieldName);

            var value = NormalizeSpaces(input.Trim());

            if (value.Length > maxLength) throw new DomainValidationException 
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, fieldName);

            LettersDigitsSpacesOnly(value, fieldName); 

            return new DisplayName(value); 
        }

        public override string ToString() => Value; // لتحويل الكلاس لنص

        // -------------------------
        //          Helpers
        // -------------------------

        // تتحقق من أن النص يحوي فقط على (حروف ,أرقام ,مسافات)د
        private static void LettersDigitsSpacesOnly(string value, string fieldName) 
        {
            bool hasLetterOrDigit = false;

            foreach (var ch in value)
            {
                if (char.IsLetterOrDigit(ch))
                {
                    hasLetterOrDigit = true;
                    continue;
                }

                if (ch == ' ') continue;

                throw new DomainValidationException
                    (ValidationErrors.InvalidFormatCode, ValidationErrors.InvalidFormatMessage, fieldName);
            }

            if (!hasLetterOrDigit) throw new DomainValidationException
                    (ValidationErrors.InvalidFormatCode, ValidationErrors.InvalidFormatMessage, fieldName);
        }

        private static string NormalizeSpaces(string input) // تنظف المسافات داخل النص
        {
            Span<char> buffer = input.Length <= 256 ? stackalloc char[input.Length] : new char[input.Length];

            int i = 0;
            bool prevSpace = false;

            foreach (var ch in input)
            {
                if (char.IsWhiteSpace(ch))
                {
                    if (prevSpace) continue;
                    buffer[i++] = ' ';
                    prevSpace = true;
                }
                else
                {
                    buffer[i++] = ch;
                    prevSpace = false;
                }
            }

            return new string(buffer[..i]).Trim();
        }

        // لمقارنة القيم المدخلة من دونه لا يمكن المقارنة لان كل قيمة تعتبر مختلفة بذاكرة
        public override bool Equals(object? obj) => obj is DisplayName other && string.Equals(Value, other.Value, StringComparison.Ordinal);

        // تعطي رقم يمثل القيمة للمقارنة
        public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value);
    }
}