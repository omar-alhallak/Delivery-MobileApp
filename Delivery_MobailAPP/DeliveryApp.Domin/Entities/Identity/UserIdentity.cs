using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.DomainExceptions;
using DeliveryApp.Domain.Enums.IdentityEnums;
using DeliveryApp.Domain.DomainErrors.IdentityErrors;

namespace DeliveryApp.Domain.Entities.Identity
{
    public class UserIdentity // يمثل وسائل تسجيل الدخول المرتبطة بالمستخدم
    {
        // -------------------------
        //            Key
        // -------------------------

        public UserIdentityID ID { get; private set; } // PK معرف الهوية
        public UserID UserID { get; private set; } // المستخدم المرتبط بهذه الهوية

        // -------------------------
        //        Provider Info
        // -------------------------

        public AuthProvider Provider { get; private set; } // مزود تسجيل الدخول
        public string? ProviderUserId { get; private set; } // معرف المستخدم من جهة تسجيل

        public string? PasswordHash { get; private set; } // كلمة المرور المشفرة

        // -------------------------
        //           Dates
        // -------------------------

        public DateTimeOffset CreatedAt { get; private set; } // وقت إنشاء الهوية

        private UserIdentity() { }

        private UserIdentity(UserIdentityID id, UserID userId, AuthProvider provider, string? providerUserId, string? passwordHash, DateTimeOffset createdAtUtc)
        {
            if (id.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(id));

            if (userId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(userId));

            if (createdAtUtc == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(createdAtUtc));

            if (!Enum.IsDefined(typeof(AuthProvider), provider)) throw new DomainValidationException
                    (UserIdentityErrors.UnsupportedProviderCode, UserIdentityErrors.UnsupportedProviderMessage, nameof(provider));

            ID = id;
            UserID = userId;
            Provider = provider;

            ProviderUserId = Normalize(providerUserId);
            PasswordHash = Normalize(passwordHash);

            CreatedAt = createdAtUtc;

            ValidateInvariants();
        }

        // -------------------------
        //          Identity
        // -------------------------

        public static UserIdentity CreateLocal(UserIdentityID id, UserID userId, string passwordHash, DateTimeOffset createdAtUtc) // إنشاء طريقة تسجيل محلية
        {
            if (string.IsNullOrWhiteSpace(passwordHash)) throw new DomainValidationException
                    (UserIdentityErrors.PasswordRequiredForLocalCode, UserIdentityErrors.PasswordRequiredForLocalMessage, nameof(passwordHash));

            return new UserIdentity(
                id: id,
                userId: userId,
                provider: AuthProvider.Local,
                providerUserId: null,
                passwordHash: passwordHash,
                createdAtUtc: createdAtUtc);
        }

        public static UserIdentity CreateGoogle(UserIdentityID id, UserID userId, string googleSub, DateTimeOffset createdAtUtc) // إنشاء طريقة تسجيل Google
        {
            if (string.IsNullOrWhiteSpace(googleSub)) throw new DomainValidationException
                    (UserIdentityErrors.GoogleSubRequiredCode, UserIdentityErrors.GoogleSubRequiredMessage, nameof(googleSub));

            return new UserIdentity(
                id: id,
                userId: userId,
                provider: AuthProvider.Google,
                providerUserId: googleSub,
                passwordHash: null,
                createdAtUtc: createdAtUtc);
        }

        public void ChangeLocalPasswordHash(string newPasswordHash) // تغيير كلمة المرور
        {
            if (Provider != AuthProvider.Local) throw new DomainRuleViolationException
                    (UserIdentityErrors.PasswordChangeOnlyForLocalCode, UserIdentityErrors.PasswordChangeOnlyForLocalMessage);

            if (string.IsNullOrWhiteSpace(newPasswordHash)) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(newPasswordHash));

            PasswordHash = Normalize(newPasswordHash);

            ValidateInvariants();
        }

        // -------------------------
        //         Validation
        // -------------------------

        private void ValidateInvariants() // التحقق من القواعد حسب طريقة التسجيل
        {
            if (ProviderUserId is not null && ProviderUserId.Length > 128) throw new DomainValidationException
                   (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, nameof(ProviderUserId));

            if (PasswordHash is not null && PasswordHash.Length > 300) throw new DomainValidationException
                   (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, nameof(PasswordHash));

            switch (Provider)
            {
                case AuthProvider.Local:
                    if (string.IsNullOrWhiteSpace(PasswordHash)) throw new DomainRuleViolationException
                            (UserIdentityErrors.PasswordRequiredForLocalCode, UserIdentityErrors.PasswordRequiredForLocalMessage);

                    if (!string.IsNullOrWhiteSpace(ProviderUserId)) throw new DomainRuleViolationException
                            (UserIdentityErrors.LocalCantBeHaveProviderUserIDCode, UserIdentityErrors.LocalCantBeHaveProviderUserIDMessage);

                    break;

                case AuthProvider.Google:
                    if (string.IsNullOrWhiteSpace(ProviderUserId)) throw new DomainRuleViolationException
                            (UserIdentityErrors.GoogleSubRequiredCode, UserIdentityErrors.GoogleSubRequiredMessage);

                    if (!string.IsNullOrWhiteSpace(PasswordHash)) throw new DomainRuleViolationException
                            (UserIdentityErrors.GoogleCantBeHavePasswordHashCode, UserIdentityErrors.GoogleCantBeHavePasswordHashMessage);

                    break;

                default:
                    throw new DomainValidationException(UserIdentityErrors.UnsupportedProviderCode, UserIdentityErrors.UnsupportedProviderMessage, nameof(Provider));
            }
        }

        // -------------------------
        //          Helpers
        // -------------------------

        private static string? Normalize(string? s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim(); // تنظيف النصوص
    }
}