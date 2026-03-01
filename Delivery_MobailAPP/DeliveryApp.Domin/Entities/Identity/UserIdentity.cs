using System;
using DeliveryApp.Domain.Enums;

namespace DeliveryApp.Domain.Entities.Identity
{

    public class UserIdentity
    {
        public UserIdentityID ID { get; private set; }
        public UserID UserID { get; private set; }

        public AuthProvider Provider { get; private set; }
        public string? ProviderUserId { get; private set; }

        public string? PasswordHash { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; }

        private UserIdentity() { }

        private UserIdentity(UserIdentityID id, UserID UserId, AuthProvider provider, string? providerUserId,
            string? passwordHash, DateTimeOffset CreatedAtUtc)
        {
            ID = id;
            UserID = UserId;
            Provider = provider;

            ProviderUserId = Normalize(providerUserId);
            PasswordHash = Normalize(passwordHash);

            CreatedAt = CreatedAtUtc;

            ValidateInvariants();
        }

        // -------------------------
        //         Identity
        // -------------------------

        public static UserIdentity CreateLocal(UserIdentityID id, UserID UserId, string passwordHash, DateTimeOffset CreatedAtUtc)
        {
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("PasswordHash is required for Local.", nameof(passwordHash));

            return new UserIdentity(id: id, UserId: UserId, provider: AuthProvider.Local, providerUserId: null,
                passwordHash: passwordHash, CreatedAtUtc: CreatedAtUtc);
        }

        public static UserIdentity CreateGoogle(UserIdentityID id, UserID UserId, string googleSub, DateTimeOffset CreatedAtUtc)
        {
            if (string.IsNullOrWhiteSpace(googleSub))
                throw new ArgumentException("GoogleSub (ProviderUserId) is required.", nameof(googleSub));

            return new UserIdentity(id: id, UserId: UserId, provider: AuthProvider.Google, providerUserId: googleSub,
                passwordHash: null, CreatedAtUtc: CreatedAtUtc);
        }

        public void ChangeLocalPasswordHash(string NewPasswordHash)
        {
            if (Provider != AuthProvider.Local)
                throw new InvalidOperationException("Password hash can be changed only for Local provider.");

            if (string.IsNullOrWhiteSpace(NewPasswordHash))
                throw new ArgumentException("PasswordHash cannot be empty.", nameof(NewPasswordHash));

            PasswordHash = Normalize(NewPasswordHash);

            ValidateInvariants();
        }

        // -------------------------
        //         Validaion
        // -------------------------

        private void ValidateInvariants()
        {
            if (ProviderUserId is not null && ProviderUserId.Length > 128)
                throw new InvalidOperationException("ProviderUserId is too long.");

            if (PasswordHash is not null && PasswordHash.Length > 300)
                throw new InvalidOperationException("PasswordHash is too long.");

            switch (Provider)
            {
                case AuthProvider.Local:
                    if (string.IsNullOrWhiteSpace(PasswordHash))
                        throw new InvalidOperationException("Local identity requires PasswordHash.");

                    if (!string.IsNullOrWhiteSpace(ProviderUserId))
                        throw new InvalidOperationException("Local identity must not have ProviderUserId.");
                    break;

                case AuthProvider.Google:
                    if (string.IsNullOrWhiteSpace(ProviderUserId))
                        throw new InvalidOperationException("Google identity requires ProviderUserId.");

                    if (!string.IsNullOrWhiteSpace(PasswordHash))
                        throw new InvalidOperationException("Google identity must not have PasswordHash.");
                    break;

                default:
                    throw new InvalidOperationException($"Unsupported provider: {Provider}");
            }
        }

        private static string? Normalize(string? s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim();
    }
}