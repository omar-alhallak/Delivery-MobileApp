using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.DomainExceptions;

namespace DeliveryApp.Domain.Entities.Drivers
{
    public class VehicleType // يمثل نوع مركبة السائق
    {
        // -------------------------
        //            Key
        // -------------------------

        public VehicleTypeID ID { get; private set; } // PK معرف نوع المركبة

        // -------------------------
        //       Vehicle Info
        // -------------------------

        public string VehicleName { get; private set; } = null!; // نوع المركبة

        // -------------------------
        //     Delivery Limits
        // -------------------------

        public double MaxDistanceKm { get; private set; } // أقصى مسافة توصيل مسموحة
        public double MaxMergeExtraKm { get; private set; } // أقصى مسافة إضافية مسموحة عند دمج الطلبات
        public int MaxOrdersToBatch { get; private set; } // أقصى عدد طلبات يمكن تجميعها معا

        // -------------------------
        //       Commission
        // -------------------------

        public int CommissionPercent { get; private set; } // نسبة العمولة لهذه المركبة

        // -------------------------
        //          Requird
        // -------------------------

        public bool RequiresLicenseAndPlate { get; private set; } // هل هذا النوع يحتاج رخصة ولوحة

        // -------------------------
        //         Status
        // -------------------------

        public bool IsActive { get; private set; } // هل نوع المركبة مفعل ويمكن استخدامه

        private VehicleType() { }

        public VehicleType(VehicleTypeID id, string name, double maxDistanceKm, double maxMergeExtraKm, int maxOrdersToBatch, int commissionPercent, bool requiresLicenseAndPlate)
        {
            if (id.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(id));

            ID = id;

            SetName(name);
            SetLimits(maxDistanceKm, maxMergeExtraKm, maxOrdersToBatch);
            SetCommission(commissionPercent);

            RequiresLicenseAndPlate = requiresLicenseAndPlate;
            IsActive = true;
        }

        // -------------------------
        //          Behavior
        // -------------------------

        public void Rename(string name) => SetName(name); // تغيير اسم نوع المركبة

        public void ChangeLimits(double maxDistanceKm, double maxMergeExtraKm, int maxOrdersToBatch) => SetLimits(maxDistanceKm, maxMergeExtraKm, maxOrdersToBatch); // تغيير حدود التوصيل

        public void ChangeCommission(int commissionPercent) => SetCommission(commissionPercent); // تغيير نسبة العمولة

        public void SetRequiresLicenseAndPlate(bool value) => RequiresLicenseAndPlate = value; // تحديد هل هذا النوع يتطلب رخصة ولوحة

        public void Activate() // تفعيل
        {
            if (IsActive) return;
            IsActive = true;
        }

        public void Deactivate() // تعطيل
        {
            if (!IsActive) return;
            IsActive = false;
        }

        public bool CanAcceptMoreOrders(int currentActiveOrders) // التحقق هل يمكن للمركبة استقبال طلبات إضافية
        {
            if (currentActiveOrders < 0) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(currentActiveOrders));

            if (!IsActive) return false;

            return currentActiveOrders < MaxOrdersToBatch;
        }

        public double GetTotalRangeLimit() => MaxDistanceKm + MaxMergeExtraKm; // حساب الحد الأقصى للمسافة

        // -------------------------
        //          Setters
        // -------------------------

        private void SetName(string value) // إدخال نوع المركبة
        {
            if (string.IsNullOrWhiteSpace(value)) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(VehicleName));

            value = value.Trim();

            if (value.Length > 100) throw new DomainValidationException
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, nameof(VehicleName));

            VehicleName = value;
        }

        private void SetLimits(double maxDistanceKm, double maxMergeExtraKm, int maxOrdersToBatch) // إدخال حدود التشغيل لهذا النوع
        {
            if (maxDistanceKm <= 0) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(MaxDistanceKm));

            if (maxMergeExtraKm < 0) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(MaxMergeExtraKm));

            if (maxOrdersToBatch < 1) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(MaxOrdersToBatch));

            MaxDistanceKm = maxDistanceKm;
            MaxMergeExtraKm = maxMergeExtraKm;
            MaxOrdersToBatch = maxOrdersToBatch;
        }

        private void SetCommission(int commissionPercent) // إدخال نسبة العمولة 
        {
            if (commissionPercent < 0 || commissionPercent > 100) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(CommissionPercent));

            CommissionPercent = commissionPercent;
        }
    }
}