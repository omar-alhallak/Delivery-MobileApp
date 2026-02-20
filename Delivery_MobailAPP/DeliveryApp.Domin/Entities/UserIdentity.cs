using DeliveryApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Domain.Entities
{
    public class UserIdentity
    {
        public Guid UserIdentityID { get; private set; }
        public Guid UserID { get; private set; }

        public AuthProvider Provider { get; private set; }

        public string? ProviderUserID { get; private set; }

        public string? PasswordHash { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; }

        private UserIdentity() { }

        private UserIdentity(Guid UserIdentityId, Guid UserId, AuthProvider provider, string? ProviderUserId, string? passwordHash, DateTimeOffset CreatedAtUtc)
        {
            if (UserIdentityId == Guid.Empty) throw new ArgumentException("UserIdentityId cannot be empty.");
            if (UserId == Guid.Empty) throw new ArgumentException("UserId cannot be empty.");

            UserIdentityID = UserIdentityId;
            UserID = UserId;
            Provider = provider;

            ProviderUserID = Normalize(ProviderUserId);
            PasswordHash = Normalize(passwordHash);

            CreatedAt = CreatedAtUtc;

            Validate();
        }

        // إنشاء Local
        public static UserIdentity CreateLocal(Guid UserIdentityId, Guid UserId, string passwordHash, DateTimeOffset CreatedAtUtc)
        {
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("PasswordHash is required for Local.");

            return new UserIdentity(UserIdentityId, UserId, AuthProvider.Local, ProviderUserId: null, passwordHash: passwordHash, CreatedAtUtc);
        }

        // إنشاء Google
        public static UserIdentity CreateGoogle(Guid UserIdentityId, Guid UserId, string GoogleSub, DateTimeOffset CreatedAtUtc)
        {
            if (string.IsNullOrWhiteSpace(GoogleSub))
                throw new ArgumentException("ProviderUserId (Google sub) is required.");

            return new UserIdentity(UserIdentityId, UserId, AuthProvider.Google, ProviderUserId: GoogleSub, passwordHash: null, CreatedAtUtc);
        }

        private void Validate()
        {
            if (Provider == AuthProvider.Local)
            {
                if (string.IsNullOrWhiteSpace(PasswordHash))
                    throw new InvalidOperationException("Local identity requires PasswordHash.");
                if (!string.IsNullOrWhiteSpace(ProviderUserID))
                    throw new InvalidOperationException("Local identity must not have ProviderUserId.");
            }
            else 
            {
                if (string.IsNullOrWhiteSpace(ProviderUserID))
                    throw new InvalidOperationException("Google identity requires ProviderUserId.");
                if (!string.IsNullOrWhiteSpace(PasswordHash))
                    throw new InvalidOperationException("Google identity must not have PasswordHash.");
            }

            if (ProviderUserID is not null && ProviderUserID.Length > 128)
                throw new InvalidOperationException("ProviderUserId too long.");
            if (PasswordHash is not null && PasswordHash.Length > 300)
                throw new InvalidOperationException("PasswordHash too long.");
        }

        private static string? Normalize(string? s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim();
    }
}