using System;

namespace DeliveryApp.Domain.Entities.Drivers
{
    public class Driver
    {
        public UserID ID { get; private set; }
        public VehicleTypeID VehicleTypeID { get; private set; }

        public bool IsEnabled { get; private set; } = true;
        public UserID? DisabledByAdminID { get; private set; }
        public DateTimeOffset? DisabledAt { get; private set; }

        public bool IsAvailable { get; private set; }
        public int ActiveOrdersCount { get; private set; }

        public DateTimeOffset? LastSeenAt { get; private set; }

        public decimal? CurrentLat { get; private set; }
        public decimal? CurrentLng { get; private set; }
        public DateTimeOffset? LastLocationAt { get; private set; }

        public UserID ApprovedByAdminID { get; private set; }
        public DateTimeOffset ApprovedAt { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }

        private Driver() { }

        public Driver(UserID UserId, VehicleTypeID VehicleTypeId, UserID ApprovedByAdminId, DateTimeOffset ApprovedAtUtc)
        {
            if (UserId.IsEmpty) throw new ArgumentException("UserId cannot be empty.", nameof(UserId));
            if (VehicleTypeId.IsEmpty) throw new ArgumentException("VehicleTypeId cannot be empty.", nameof(VehicleTypeId));
            if (ApprovedByAdminId.IsEmpty) throw new ArgumentException("ApprovedByAdminId cannot be empty.", nameof(ApprovedByAdminId));

            ID = UserId;
            VehicleTypeID = VehicleTypeId;

            ApprovedByAdminID = ApprovedByAdminId;
            ApprovedAt = ApprovedAtUtc;
            CreatedAt = ApprovedAtUtc;

            IsAvailable = false;
            ActiveOrdersCount = 0;
            IsEnabled = true;
        }

        // -------------------------------
        //      disable/enable driver
        // -------------------------------
        public void Disable(UserID AdminId, DateTimeOffset UtcNow)
        {
            if (AdminId.IsEmpty) throw new ArgumentException("AdminId cannot be empty.", nameof(AdminId));
            if (!IsEnabled) return;

            IsEnabled = false;
            IsAvailable = false;

            DisabledByAdminID = AdminId;
            DisabledAt = UtcNow;
        }

        public void Enable(UserID AdminId, DateTimeOffset UtcNow)
        {
            if (AdminId.IsEmpty) throw new ArgumentException("AdminId cannot be empty.", nameof(AdminId));
            if (IsEnabled) return;

            IsEnabled = true;
            DisabledByAdminID = null;
            DisabledAt = null;
        }

        private void IsNotDisable()
        {
            if (!IsEnabled)
                throw new InvalidOperationException("Driver app access is disabled.");
        }

        // ---------- Online ----------
        public bool IsOnline(DateTimeOffset UtcNow, TimeSpan threshold)
        {
            if (threshold <= TimeSpan.Zero)
                throw new ArgumentException("Threshold must be positive.", nameof(threshold));

            return LastSeenAt.HasValue && (UtcNow - LastSeenAt.Value) <= threshold;
        }

        // -------------------------
        //   Activity -- Location
        // -------------------------
        public void Touch(DateTimeOffset UtcNow)
        {
            IsNotDisable();
            LastSeenAt = UtcNow;
        }

        public void UpdateLocation(decimal lat, decimal lng, DateTimeOffset UtcNow)
        {
            IsNotDisable();
            ValidateLocation(lat, lng);

            CurrentLat = lat;
            CurrentLng = lng;
            LastLocationAt = UtcNow;
            LastSeenAt = UtcNow;
        }

        // ---------- Availability ----------
        public void SetAvailable(bool available, DateTimeOffset UtcNow, TimeSpan onlineThreshold)
        {
            IsNotDisable();

            if (available)
                if (!IsOnline(UtcNow, onlineThreshold))
                {
                    throw new InvalidOperationException("Driver is offline.");
                }
                else
                {
                    if (ActiveOrdersCount > 0)
                        throw new InvalidOperationException("Cannot go unavailable while having active orders.");
                }

            IsAvailable = available;
        }

        // --------- Vehicle ---------
        public void ChangeVehicle(VehicleTypeID NewVehicleTypeId)
        {
            IsNotDisable();

            if (NewVehicleTypeId.IsEmpty)
                throw new ArgumentException("New vehicle type is required.", nameof(NewVehicleTypeId));

            VehicleTypeID = NewVehicleTypeId;
        }

        // -------------------------
        //      Orders counter
        // -------------------------
        public void AssignOrders()
        {
            IsNotDisable();
            ActiveOrdersCount++;
        }

        public void CompletOrders()
        {
            IsNotDisable();
            if (ActiveOrdersCount <= 0) return;
            ActiveOrdersCount--;
        }

        private static void ValidateLocation(decimal lat, decimal lng)
        {
            if (lat < -90 || lat > 90)
                throw new ArgumentOutOfRangeException(nameof(lat), "Latitude must be between -90 and 90.");
            if (lng < -180 || lng > 180)
                throw new ArgumentOutOfRangeException(nameof(lng), "Longitude must be between -180 and 180.");
        }
    }
}