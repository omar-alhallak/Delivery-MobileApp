using System;
using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.ValueObjects;
using DeliveryApp.Domain.DomainExceptions;
using DeliveryApp.Domain.DomainErrors.DriverErrors;
using DeliveryApp.Domain.Enums.EngagementEnams;

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

        public GeoPoint? CurrentLocation { get; private set; }
        public DateTimeOffset? LastLocationAt { get; private set; }

        public decimal AverageRating { get; private set; }
        public int RatingsCount { get; private set; }

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

            AverageRating = 0;
            RatingsCount = 0;

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
            if (threshold <= TimeSpan.Zero) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(threshold));

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

            CurrentLocation = GeoPoint.Create(lat, lng);
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

        // -------------------------
        //         Rating
        // -------------------------

        public void AddRating(RatingStars stars)
        {
            ValidateRatingStars(stars);

            var value = (int)stars;

            AverageRating = ((AverageRating * RatingsCount) + value) / (RatingsCount + 1);
            RatingsCount++;
        }

        public void UpdateRating(RatingStars oldStars, RatingStars newStars)
        {
            ValidateRatingStars(oldStars);
            ValidateRatingStars(newStars);

            if (RatingsCount <= 0) throw new DomainRuleViolationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage);

            var oldValue = (int)oldStars;
            var newValue = (int)newStars;

            AverageRating = ((AverageRating * RatingsCount) - oldValue + newValue) / RatingsCount;
        }

        private static void ValidateRatingStars(RatingStars stars)
        {
            if (!Enum.IsDefined(typeof(RatingStars), stars)) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(stars));
        }
    }
}