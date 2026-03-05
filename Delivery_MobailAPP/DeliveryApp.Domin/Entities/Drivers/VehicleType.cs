using System;
using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.DomainExceptions;

namespace DeliveryApp.Domain.Entities.Drivers
{
    public class VehicleType
    {
        public VehicleTypeID ID { get; private set; }
        public string VehicleName { get; private set; } = null!;

        public double MaxDistanceKm { get; private set; }
        public double MaxMergeExtraKm { get; private set; }
        public int MaxOrdersToBatch { get; private set; }
        public int CommissionPercent { get; private set; }

        public bool RequiresLicenseAndPlate { get; private set; }

        public bool IsActive { get; private set; }

        private VehicleType() { }

        public VehicleType(VehicleTypeID id, string name, double maxDist, double maxExtra, int maxBatch,
            int commissionPercent, bool requiresLicenseAndPlate)
        {
            if (id.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(ID));

            ID = id;

            SetName(name);
            SetLimits(maxDist, maxExtra, maxBatch);
            SetCommission(commissionPercent);

            RequiresLicenseAndPlate = requiresLicenseAndPlate;

            IsActive = true;
        }

        // -------------------------
        //          Behavior
        // -------------------------

        public void Rename(string name) => SetName(name);

        public void ChangeLimits(double maxDist, double maxExtra, int maxBatch) => SetLimits(maxDist, maxExtra, maxBatch);

        public void ChangeCommission(int commissionPercent) => SetCommission(commissionPercent);

        public void SetRequiresLicenseAndPlate(bool value) => RequiresLicenseAndPlate = value;

        public void Activate() => IsActive = true;
        public void Deactivate() => IsActive = false;

        public bool CanAcceptMoreOrders(int currentActiveOrders)
        {
            if (!IsActive) return false;
            if (currentActiveOrders < 0) currentActiveOrders = 0;
            return currentActiveOrders < MaxOrdersToBatch;
        }

        public double GetTotalRangeLimit() => MaxDistanceKm + MaxMergeExtraKm;

        // -------------------------
        //          setters
        // -------------------------

        private void SetName(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(VehicleName));

            value = value.Trim();

            if (value.Length > 100) throw new DomainValidationException
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, nameof(VehicleName));

            VehicleName = value;
        }

        private void SetLimits(double maxDist, double maxExtra, int maxBatch)
        {
            if (maxDist <= 0) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(MaxDistanceKm));

            if (maxExtra < 0) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(MaxMergeExtraKm));

            if (maxBatch < 1) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(MaxOrdersToBatch));

            MaxDistanceKm = maxDist;
            MaxMergeExtraKm = maxExtra;
            MaxOrdersToBatch = maxBatch;
        }

        private void SetCommission(int commissionPercent)
        {
            if (commissionPercent < 0 || commissionPercent > 100) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(CommissionPercent));

            CommissionPercent = commissionPercent;
        }
    }
}