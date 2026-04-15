using System.Security.Cryptography;
using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.DomainExceptions;
using DeliveryApp.Domain.Enums.IdentityEnums;
using DeliveryApp.Domain.DomainErrors.IdentityErrors;

namespace DeliveryApp.Domain.Entities.Identity
{
    // -------------------------
    //         شرح توكين
    // -------------------------
    // عند تسجيل الدخول نظام يعطي المستخدم شيئين :
    // Access Tocen : قصير العمر للطلبات اليومية
    // Refresh Token : طويل العمر يستخدم لتوليد أكسس توكين بدل من تسجيل الدخول كل مرة

    // HMACSHA256 : طريقة تشفير متل الهاش لكن أمن أكثر وأسرع يستخدم خصيصا مع توكين

    public class UserSession // يمثل جلسة تسجيل دخول واحدة للمستخدم على كل تطبيق من المنصة
    {
        // -------------------------
        //         Constants
        // -------------------------

        private const int RefreshHashLength = 32; // HMACSHA256
        private const int MaxDeviceIdLength = 100; // أقصى طول لمعرف جهاز

        // -------------------------
        //            Key 
        // -------------------------

        public UserSessionID ID { get; private set; } // معرف الجلسة
        public UserID UserID { get; private set; } // صاحب الجلسة

        // -------------------------
        //        Client Info
        // -------------------------
        public ClientType ClientType { get; private set; } // نوع التطبيق الي تابعة له جلسة
        public string DeviceID { get; private set; } = null!; // معرف الجهاز

        // -------------------------
        //    Refresh Token Hash
        // -------------------------

        private byte[] refreshTokenHash = Array.Empty<byte>(); // الهاش داخلي ل Refresh token

        // -------------------------
        //           Dates 
        // -------------------------

        public DateTimeOffset CreatedAt { get; private set; } // وقت إنشاء الجلسة
        public DateTimeOffset LastSeenAt { get; private set; } // آخر وقت كانت الجلسة فيه نشطة
        public DateTimeOffset ExpiresAt { get; private set; } // وقت انتهاء الجلسة

        public DateTimeOffset? RevokedAt { get; private set; } // وقت إلغاء الجلسة
        public bool IsRevoked => RevokedAt.HasValue; // هل جلسة ملغاة

        private UserSession() { }

        private UserSession(UserSessionID id, UserID userId, ClientType clientType, string deviceId, byte[] refreshTokenHash, DateTimeOffset createdAtUtc, DateTimeOffset expiresAtUtc)
        {
            if (id.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(id));

            if (userId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(userId));

            if (!Enum.IsDefined(typeof(ClientType), clientType)) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(clientType));

            if (createdAtUtc == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(createdAtUtc));

            if (expiresAtUtc <= createdAtUtc) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(expiresAtUtc));

            ID = id;
            UserID = userId;
            ClientType = clientType;

            DeviceID = NormalizeAndValidateDeviceId(deviceId);
            StoreRefreshTokenHash(refreshTokenHash);

            CreatedAt = createdAtUtc;
            LastSeenAt = createdAtUtc;
            ExpiresAt = expiresAtUtc;

            ValidateState();
        }

        // -------------------------
        //          Factory
        // -------------------------

        public static UserSession Create(UserSessionID id, UserID userId, ClientType clientType, string deviceId, byte[] refreshTokenHash, DateTimeOffset utcNow, TimeSpan lifetime) // إنشاء جلسة جديدة
        {
            if (lifetime <= TimeSpan.Zero) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(lifetime));

            if (utcNow == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(utcNow));

            return new UserSession(
                id: id,
                userId: userId,
                clientType: clientType,
                deviceId: deviceId,
                refreshTokenHash: refreshTokenHash,
                createdAtUtc: utcNow,
                expiresAtUtc: utcNow.Add(lifetime));
        }

        // -------------------------
        //        Device Rule
        // -------------------------

        public void EnsureSameDevice(string deviceId) // تأكد من أن العملية تمت من نفس جهاز
        {
            var normalized = NormalizeAndValidateDeviceId(deviceId);

            if (!string.Equals(DeviceID, normalized, StringComparison.Ordinal)) throw new DomainConflictException
                    (UserSessionErrors.DeviceMismatchCode, UserSessionErrors.DeviceMismatchMessage);
        }

        // -------------------------
        //          Expiry
        // -------------------------

        public void Refresh(string deviceId, byte[] newRefreshTokenHash, DateTimeOffset utcNow, TimeSpan lifetime) // تحديث الجلسة
        {
            if (utcNow == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(utcNow));

            EnsureSameDevice(deviceId);
            EnsureActive(utcNow);

            if (lifetime <= TimeSpan.Zero) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(lifetime));

            StoreRefreshTokenHash(newRefreshTokenHash);

            LastSeenAt = utcNow;
            ExpiresAt = utcNow.Add(lifetime); // Sliding expiration: كل refresh يمدد الجلسة

            ValidateState();
        }

        public void Revoke(DateTimeOffset utcNow) // إلغاء جلسة نهائياً
        {
            if (IsRevoked) return;

            if (utcNow == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(utcNow));

            if (utcNow < CreatedAt) throw new DomainValidationException
                    (UserSessionErrors.RevokedAtBeforeCreatedAtCode, UserSessionErrors.RevokedAtBeforeCreatedAtMessage, nameof(utcNow));

            RevokedAt = utcNow;
            LastSeenAt = utcNow;
        }

        // -------------------------
        //           State
        // -------------------------

        public bool IsExpired(DateTimeOffset utcNow) // هل إنتهى صلاحية الجلسة
        {
            if (utcNow == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(utcNow));

            return utcNow >= ExpiresAt;
        }

        public bool IsActive(DateTimeOffset utcNow) // هل جلسة ما تزال فعالة
        {
            if (utcNow == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(utcNow));

            return !IsRevoked && !IsExpired(utcNow);
        }

        public bool MatchesRefreshTokenHash(byte[] hash) // التحقق من تطابق هاش ال Refresh token
        {
            if (hash is null) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(hash));

            if (hash.Length != RefreshHashLength) return false;

            return CryptographicOperations.FixedTimeEquals(refreshTokenHash, hash);
        }

        private void EnsureActive(DateTimeOffset utcNow) // تأكد أن جلسة ما تزال فعالة
        {
            if (IsRevoked) throw new DomainRuleViolationException
                    (UserSessionErrors.SessionRevokedCode, UserSessionErrors.SessionRevokedMessage);

            if (IsExpired(utcNow)) throw new DomainRuleViolationException
                    (UserSessionErrors.SessionExpiredCode, UserSessionErrors.SessionExpiredMessage);
        }

        // -------------------------
        //          Helpers
        // -------------------------

        private void StoreRefreshTokenHash(byte[] hash) // تخزين هاش لل Refresh token
        {
            if (hash is null) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(hash));

            if (hash.Length != RefreshHashLength) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(hash));

            refreshTokenHash = (byte[])hash.Clone();
        }

        private static string NormalizeAndValidateDeviceId(string deviceId) // تنظيف وتحقق من معرف جهاز
        {
            if (string.IsNullOrWhiteSpace(deviceId)) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(deviceId));

            deviceId = deviceId.Trim();

            if (deviceId.Length > MaxDeviceIdLength) throw new DomainValidationException
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, nameof(deviceId));

            return deviceId;
        }

        private void ValidateState() // التأكد أن حالة الجلسة صحيحة
        {
            if (CreatedAt == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(CreatedAt));

            if (LastSeenAt < CreatedAt) throw new DomainRuleViolationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage);

            if (ExpiresAt <= CreatedAt) throw new DomainRuleViolationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage);

            if (refreshTokenHash.Length != RefreshHashLength) throw new DomainRuleViolationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage);

            if (string.IsNullOrWhiteSpace(DeviceID)) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(DeviceID));

            if (RevokedAt.HasValue && RevokedAt.Value < CreatedAt) throw new DomainValidationException
                    (UserSessionErrors.RevokedAtBeforeCreatedAtCode, UserSessionErrors.RevokedAtBeforeCreatedAtMessage, nameof(RevokedAt));
        }
    }
} 