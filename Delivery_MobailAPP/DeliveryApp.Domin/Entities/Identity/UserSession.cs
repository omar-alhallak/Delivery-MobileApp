using System;
using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.DomainExceptions;
using DeliveryApp.Domain.DomainErrors.IdentityErrors;

namespace DeliveryApp.Domain.Entities.Identity
{
    public class UserSession
    {
        private const int RefreshHashLength = 32;    // HMACSHA256 = 32
        private const int MaxDeviceIdLength = 100;

        public UserSessionID ID { get; private set; }
        public UserID UserID { get; private set; }
        public string DeviceID { get; private set; } = null!;

        private byte[] refreshTokenHash = Array.Empty<byte>();
        public ReadOnlyMemory<byte> RefreshTokenHash => refreshTokenHash;

        public DateTimeOffset CreatedAt { get; private set; }
        public DateTimeOffset LastSeenAt { get; private set; }
        public DateTimeOffset ExpiresAt { get; private set; }
        public DateTimeOffset? RevokedAt { get; private set; }
        public bool IsRevoked => RevokedAt.HasValue;

        private UserSession() { }

        private UserSession(UserSessionID id, UserID UserId, string DeviceId, byte[] refreshTokenHash,
            DateTimeOffset CreatedAtUtc, DateTimeOffset ExpiresAtUtc)
        {
            ID = id;
            UserID = UserId;

            DeviceID = NormalizeAndValidateDeviceId(DeviceId);
            StoreRefreshTokenHash(refreshTokenHash);

            CreatedAt = CreatedAtUtc;
            LastSeenAt = CreatedAtUtc;
            ExpiresAt = ExpiresAtUtc;

            ValidateState();
        }

        public static UserSession Create(UserSessionID id, UserID userId, string deviceId, byte[] refreshTokenHash,
            DateTimeOffset utcNow, TimeSpan lifetime)
        {
            if (lifetime <= TimeSpan.Zero) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(lifetime));

            return new UserSession(id: id, UserId: userId, DeviceId: deviceId, refreshTokenHash: refreshTokenHash,
                CreatedAtUtc: utcNow, ExpiresAtUtc: utcNow.Add(lifetime));
        }

        // -------------------------  
        //        Device Rule   
        // -------------------------  

        public void JustSameDevice(string DeviceId)
        {
            var normalized = NormalizeAndValidateDeviceId(DeviceId);

            if (!string.Equals(DeviceID, normalized, StringComparison.Ordinal))
                throw new DomainConflictException
                    (UserSessionErrors.DeviceMismatchCode, UserSessionErrors.DeviceMismatchMessage);
        }

        // ------------------------
        //          Expiry  
        // ------------------------

        public void Refresh(string DeviceId, byte[] newRefreshTokenHash, DateTimeOffset UtcNow, TimeSpan lifetime)
        {
            JustSameDevice(DeviceId);
            SureActive(UtcNow);

            if (lifetime <= TimeSpan.Zero) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(lifetime));

            StoreRefreshTokenHash(newRefreshTokenHash);

            LastSeenAt = UtcNow;
            ExpiresAt = UtcNow.Add(lifetime); // Sliding 

            ValidateState();
        }

        public void Revoke(DateTimeOffset UtcNow) // إلغاء للأدمن
        {
            if (IsRevoked) return;

            if (UtcNow < CreatedAt) throw new DomainValidationException
                    (UserSessionErrors.RevokedAtBeforeCreatedAtCode, UserSessionErrors.RevokedAtBeforeCreatedAtMessage, nameof(UtcNow));

            RevokedAt = UtcNow;
            LastSeenAt = UtcNow;
        }

        // -------------------------  
        //           State  
        // -------------------------  

        public bool IsExpired(DateTimeOffset UtcNow) => UtcNow >= ExpiresAt;

        public bool IsActive(DateTimeOffset UtcNow) => !IsRevoked && !IsExpired(UtcNow);

        private void SureActive(DateTimeOffset UtcNow)
        {
            if (IsRevoked) throw new DomainRuleViolationException
                    (UserSessionErrors.SessionRevokedCode, UserSessionErrors.SessionRevokedMessage);

            if (IsExpired(UtcNow)) throw new DomainRuleViolationException
                    (UserSessionErrors.SessionExpiredCode, UserSessionErrors.SessionExpiredMessage);
        }

        // -------------------------  
        //          Helpers  
        // -------------------------  

        private void StoreRefreshTokenHash(byte[] hash)
        {
            if (hash is null) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(hash));
            if (hash.Length != RefreshHashLength) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(hash));

            refreshTokenHash = (byte[])hash.Clone();  
        }

        private static string NormalizeAndValidateDeviceId(string deviceId)
        {
            if (string.IsNullOrWhiteSpace(deviceId)) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(deviceId));

            deviceId = deviceId.Trim();

            if (deviceId.Length > MaxDeviceIdLength) throw new DomainValidationException
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, nameof(deviceId));

            return deviceId;
        }

        private void ValidateState()
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