using System;
using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.DomainExceptions;

namespace DeliveryApp.Domain.ValueObjects
{
    public sealed class CatalogName
    {
        public string Value { get; }

        private CatalogName(string value) => Value = value;

        public static CatalogName Create(string? input, int maxLength, string field)
        {
            if (maxLength <= 0) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(maxLength));

            if (string.IsNullOrWhiteSpace(input)) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, field);

            var value = NormalizeSpaces(input.Trim());

            if (value.Length > maxLength) throw new DomainValidationException
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, field);

            LettersDigitsSpacesOnly(value, field);

            return new CatalogName(value);
        }

        public override string ToString() => Value;

        // -------------------------
        //          Helpers
        // -------------------------
        private static void LettersDigitsSpacesOnly(string value, string field)
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

                throw new DomainValidationException(
                    ValidationErrors.InvalidFormatCode, ValidationErrors.InvalidFormatMessage, field);
            }

            if (!hasLetterOrDigit) throw new DomainValidationException
                    (ValidationErrors.InvalidFormatCode, ValidationErrors.InvalidFormatMessage, field);
        }

        private static string NormalizeSpaces(string input)
        {
            Span<char> buffer = input.Length <= 256 ? stackalloc char[input.Length] : new char[input.Length];

            int j = 0;
            bool prevSpace = false;

            foreach (var ch in input)
            {
                if (char.IsWhiteSpace(ch))
                {
                    if (prevSpace) continue;
                    buffer[j++] = ' ';
                    prevSpace = true;
                }
                else
                {
                    buffer[j++] = ch;
                    prevSpace = false;
                }
            }

            return new string(buffer[..j]).Trim();
        }

        public override bool Equals(object? obj)
            => obj is CatalogName other && string.Equals(Value, other.Value, StringComparison.Ordinal);

        public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value);
    }
}