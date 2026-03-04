using System;
using DeliveryApp.Domain.Enums;
using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.DomainExceptions;
using DeliveryApp.Domain.DomainErrors.IdentityErrors;

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
            if (id.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, field: nameof(id));

            if (UserId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, field: nameof(UserId));

            if (CreatedAtUtc == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, field: nameof(CreatedAtUtc));

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
            if (string.IsNullOrWhiteSpace(passwordHash)) throw new DomainValidationException
                    (UserIdentityErrors.PasswordRequiredForLocalCode, UserIdentityErrors.PasswordRequiredForLocalMessage, field: nameof(passwordHash));

            return new UserIdentity(id: id, UserId: UserId, provider: AuthProvider.Local, providerUserId: null,
                passwordHash: passwordHash, CreatedAtUtc: CreatedAtUtc);
        }

        public static UserIdentity CreateGoogle(UserIdentityID id, UserID UserId, string googleSub, DateTimeOffset CreatedAtUtc)
        {
            if (string.IsNullOrWhiteSpace(googleSub)) throw new DomainValidationException
                    (UserIdentityErrors.GoogleSubRequiredCode, UserIdentityErrors.GoogleSubRequiredMessage, field: nameof(googleSub));

            return new UserIdentity(id: id, UserId: UserId, provider: AuthProvider.Google, providerUserId: googleSub,
                passwordHash: null, CreatedAtUtc: CreatedAtUtc);
        }

        public void ChangeLocalPasswordHash(string NewPasswordHash)
        {
            if (Provider != AuthProvider.Local) throw new DomainRuleViolationException
                    (UserIdentityErrors.PasswordChangeOnlyForLocalCode, UserIdentityErrors.PasswordChangeOnlyForLocalMessage);

            if (string.IsNullOrWhiteSpace(NewPasswordHash)) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, field: nameof(NewPasswordHash));

            PasswordHash = Normalize(NewPasswordHash);

            ValidateInvariants();
        }

        // -------------------------
        //         Validaion
        // -------------------------

        private void ValidateInvariants()
        {
            if (ProviderUserId is not null && ProviderUserId.Length > 128)
                throw new DomainValidationException
                   (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, nameof(ProviderUserId));

            if (PasswordHash is not null && PasswordHash.Length > 300)
                throw new DomainValidationException
                   (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, nameof(PasswordHash));

            switch (Provider)
            {
                case AuthProvider.Local:
                    if (string.IsNullOrWhiteSpace(PasswordHash))
                        throw new DomainRuleViolationException
                            (UserIdentityErrors.PasswordRequiredForLocalCode, UserIdentityErrors.PasswordRequiredForLocalMessage);

                    if (!string.IsNullOrWhiteSpace(ProviderUserId))
                        throw new DomainRuleViolationException
                            (UserIdentityErrors.LocalCantBeHaveProviderUserIDCode, UserIdentityErrors.LocalCantBeHaveProviderUserIDMessage);

                    break;

                case AuthProvider.Google:
                    if (string.IsNullOrWhiteSpace(ProviderUserId))
                        throw new DomainRuleViolationException
                            (UserIdentityErrors.GoogleSubRequiredCode, UserIdentityErrors.GoogleSubRequiredMessage);


                    if (!string.IsNullOrWhiteSpace(PasswordHash))
                        throw new DomainRuleViolationException
                            (UserIdentityErrors.GoogleCantBeHavePasswordHashCode, UserIdentityErrors.GoogleCantBeHavePasswordHashMessage);
                    break;

                default:
                    throw new DomainValidationException
                            (UserIdentityErrors.UnsupportedProviderCode, UserIdentityErrors.UnsupportedProviderMessage, field: nameof(Provider));
            }
        }

        private static string? Normalize(string? s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim();
    }
}