using System;
using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.DomainExceptions;
using DeliveryApp.Domain.DomainErrors.DriverErrors;

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
            if (UserId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(UserId));

            if (VehicleTypeId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(VehicleTypeId));

            if (ApprovedByAdminId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(ApprovedByAdminId));

            if (ApprovedAtUtc == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(ApprovedAtUtc));

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
            if (AdminId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(AdminId));

            if (!IsEnabled) return;

            IsEnabled = false;
            IsAvailable = false;

            DisabledByAdminID = AdminId;
            DisabledAt = UtcNow;
        }

        public void Enable(UserID AdminId, DateTimeOffset UtcNow)
        {
            if (AdminId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(AdminId));

            if (IsEnabled) return;

            IsEnabled = true;
            DisabledByAdminID = null;
            DisabledAt = null;
        }

        private void IsNotDisable()
        {
            if (!IsEnabled) throw new DomainRuleViolationException(DriverErrors.DisabledCode, DriverErrors.DisabledMessage);
        }

        // ---------- Online ----------
        public bool IsOnline(DateTimeOffset UtcNow, TimeSpan threshold)
        {
            if (threshold <= TimeSpan.Zero)
                throw new DomainValidationException(ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(threshold));

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
            {
                if (!IsOnline(UtcNow, onlineThreshold)) throw new DomainRuleViolationException
                        (DriverErrors.OfflineCode, DriverErrors.OfflineMessage);
            }
            else
            {
                if (ActiveOrdersCount > 0) throw new DomainRuleViolationException
                        (DriverErrors.CantbeUnavailableWithActiveOrdersCode, DriverErrors.CantbeUnavailableWithActiveOrdersMessage);
            }

            IsAvailable = available;
        }

        // --------- Vehicle ---------
        public void ChangeVehicle(VehicleTypeID NewVehicleTypeId)
        {
            IsNotDisable();

            if (NewVehicleTypeId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(NewVehicleTypeId));

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
            if (lat < -90 || lat > 90) throw new DomainValidationException
                    (ValidationErrors.InvalidLatCode, ValidationErrors.InvalidLatMessage, nameof(lat));

            if (lng < -180 || lng > 180) throw new DomainValidationException
                    (ValidationErrors.InvalidLngCode, ValidationErrors.InvalidLngMessage, nameof(lng));
        }
    }
}