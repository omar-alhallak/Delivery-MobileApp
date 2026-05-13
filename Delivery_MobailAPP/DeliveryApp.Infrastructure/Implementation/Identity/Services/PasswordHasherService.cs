using Microsoft.AspNetCore.Identity;
using DeliveryApp.Application.Interfaces.IdentityInterfaces;

namespace DeliveryApp.Infrastructure.Implementation.Identity.Services
{
    public sealed class PasswordHasherService : IPasswordHasher // تشفير كلمة السر قبل التخزين
    {
        private readonly PasswordHasher<object> PasswordHasher = new();

        public string Hash(string password) // تخزين الباسوورد بالقاعدة ك Hash
        {
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Password is required.", nameof(password));

            return PasswordHasher.HashPassword(null!, password);
        }

        public bool Verify(string password, string passwordHash) // عند تسجيل تحقق أنها صحيحة
        {
            if (string.IsNullOrWhiteSpace(password)) return false;

            if (string.IsNullOrWhiteSpace(passwordHash)) return false;

            var result = PasswordHasher.VerifyHashedPassword(null!, passwordHash, password);

            return result == PasswordVerificationResult.Success ||
                   result == PasswordVerificationResult.SuccessRehashNeeded;
        }
    }
}