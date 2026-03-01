using System;

namespace DeliveryApp.Domain.Entities.Identity
{
    public class UserSession
    {
        private const int RefreshHashLength = 32;      // HMACSHA256 = 32
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
            if (lifetime <= TimeSpan.Zero)
                throw new ArgumentException("Lifetime must be positive.", nameof(lifetime));

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
                throw new InvalidOperationException("Account is already logged in on another device.");
        }

        // ------------------------
        //          Expiry  
        // ------------------------

        public void Refresh(string DeviceId, byte[] newRefreshTokenHash, DateTimeOffset UtcNow, TimeSpan lifetime)
        {
            JustSameDevice(DeviceId);
            SureActive(UtcNow);

            if (lifetime <= TimeSpan.Zero)
                throw new ArgumentException("Lifetime must be positive.", nameof(lifetime));

            StoreRefreshTokenHash(newRefreshTokenHash);

            LastSeenAt = UtcNow;
            ExpiresAt = UtcNow.Add(lifetime); // Sliding 

            ValidateState();
        }

        public void Revoke(DateTimeOffset UtcNow) // إلغاء للأدمن
        {
            if (IsRevoked) return;

            if (UtcNow < CreatedAt)
                throw new InvalidOperationException("RevokedAt cannot be earlier than CreatedAt.");

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
            if (IsRevoked)
                throw new InvalidOperationException("Session is revoked. Please login again.");

            if (IsExpired(UtcNow))
                throw new InvalidOperationException("Session is expired. Please login again.");
        }

        // -------------------------  
        //          Helpers  
        // -------------------------  

        private void StoreRefreshTokenHash(byte[] hash)
        {
            if (hash is null) throw new ArgumentNullException(nameof(hash));
            if (hash.Length != RefreshHashLength)
                throw new ArgumentException($"RefreshTokenHash must be {RefreshHashLength} bytes.", nameof(hash));

            refreshTokenHash = (byte[])hash.Clone();  
        }

        private static string NormalizeAndValidateDeviceId(string deviceId)
        {
            if (string.IsNullOrWhiteSpace(deviceId))
                throw new ArgumentException("DeviceId is required.", nameof(deviceId));

            deviceId = deviceId.Trim();

            if (deviceId.Length > MaxDeviceIdLength)
                throw new ArgumentException($"DeviceId is too long (max {MaxDeviceIdLength}).", nameof(deviceId));

            return deviceId;
        }

        private void ValidateState()
        {
            if (CreatedAt == default)
                throw new InvalidOperationException("CreatedAt is required.");

            if (LastSeenAt < CreatedAt)
                throw new InvalidOperationException("LastSeenAt cannot be earlier than CreatedAt.");

            if (ExpiresAt <= CreatedAt)
                throw new InvalidOperationException("ExpiresAt must be later than CreatedAt.");

            if (refreshTokenHash.Length != RefreshHashLength)
                throw new InvalidOperationException("RefreshTokenHash length is invalid.");

            if (string.IsNullOrWhiteSpace(DeviceID))
                throw new InvalidOperationException("DeviceId is required.");

            if (RevokedAt.HasValue && RevokedAt.Value < CreatedAt)
                throw new InvalidOperationException("RevokedAt cannot be earlier than CreatedAt.");
        }
    }
}