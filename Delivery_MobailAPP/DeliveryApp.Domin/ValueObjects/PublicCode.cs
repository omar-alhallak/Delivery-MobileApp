using System;
using System.Text.RegularExpressions;

namespace DeliveryApp.Domain.ValueObjects
{
    public readonly record struct PublicCode // ينشئ الكود الظاهر للمستخدم
    {
        private static readonly Regex Format = new(@"^[A-Z]{1,5}-\d{6}$", RegexOptions.Compiled);

        public string Value { get; }

        private PublicCode(string value)
        {
            Value = value;
        }

        public static PublicCode From(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Public code cannot be empty.", nameof(value));

            value = value.Trim().ToUpperInvariant();

            if (!Format.IsMatch(value))
                throw new ArgumentException("Invalid public code format. Example: U-000001 or ORD-000001", nameof(value));

            return new PublicCode(value);
        }

        public static PublicCode Create(string prefix, long number)
        {
            if (string.IsNullOrWhiteSpace(prefix))
                throw new ArgumentException("Prefix is required.", nameof(prefix));

            prefix = prefix.Trim().ToUpperInvariant();

            if (!Regex.IsMatch(prefix, @"^[A-Z]{1,5}$"))
                throw new ArgumentException("Prefix must contain only letters (1-5).", nameof(prefix));

            if (number <= 0)
                throw new ArgumentException("Number must be greater than zero.", nameof(number));

            var formatted = $"{prefix}-{number:000000}";
            return new PublicCode(formatted);
        }

        public override string ToString() => Value;
    }
}