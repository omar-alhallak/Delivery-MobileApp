using System.Text;
using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.DomainExceptions;

namespace DeliveryApp.Domain.ValueObjects
{
    public sealed class Slug
    {
        public string Value { get; }

        private Slug(string value)
        {
            Value = value;
        }

        public static Slug Create(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, field: nameof(Slug));

            var normalized = Normalize(input);

            if (normalized.Length > 80) throw new DomainValidationException
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, field: nameof(Slug));

            return new Slug(normalized);
        }

        private static string Normalize(string value)
        {
            value = value.Trim().ToLowerInvariant();

            var builder = new StringBuilder();
            bool lastDash = false;

            foreach (var ch in value)
            {
                if (char.IsLetterOrDigit(ch) || IsArabic(ch))
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

            if (string.IsNullOrWhiteSpace(result)) throw new DomainValidationException(
                ValidationErrors.InvalidSlugCode, ValidationErrors.InvalidSlugMessage, field: nameof(Slug));

            return result;
        }

        private static bool IsArabic(char c)
        {
            return c >= '\u0600' && c <= '\u06FF';
        }

        public override string ToString() => Value;
    }
}