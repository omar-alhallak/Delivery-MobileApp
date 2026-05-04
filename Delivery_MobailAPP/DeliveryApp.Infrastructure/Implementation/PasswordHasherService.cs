using Microsoft.AspNetCore.Identity;
using DeliveryApp.Application.Interfaces.IdentityInterfaces;

namespace DeliveryApp.Infrastructure.Implementation
{
    public sealed class PasswordHasherService : IPasswordHasher
    {
        private readonly PasswordHasher<object> _passwordHasher = new();

        public string Hash(string password)
        {
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Password is required.", nameof(password));

            return _passwordHasher.HashPassword(null!, password);
        }

        public bool Verify(string password, string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(password)) return false;

            if (string.IsNullOrWhiteSpace(passwordHash)) return false;

            var result = _passwordHasher.VerifyHashedPassword(null!, passwordHash, password);

            return result == PasswordVerificationResult.Success ||
                   result == PasswordVerificationResult.SuccessRehashNeeded;
        }
    }
}