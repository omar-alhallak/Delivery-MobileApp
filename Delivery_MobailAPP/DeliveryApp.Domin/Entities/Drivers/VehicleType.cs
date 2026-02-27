using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Domain.Entities.Drivers
{
    public class VehicleType
    {

        // -------------------------
        // Properties (Private Setters)
        // -------------------------
        public int VehicleID { get; private set; }
        public string VehicleName { get; private set; } = default!;

        // الحد الأقصى للمسافة المسموحة للطلب الواحد لهذه المركبة
        public double MaxDistanceKm { get; private set; }

        // المسافة الإضافية المسموحة عند دمج طلبين (Batching)
        public double MaxMergeExtraKm { get; private set; }

        // نسبة عمولة المنصة من الطلب (مثلاً 0.15 تعني 15%)
        public double CommissionPercent { get; private set; }

        public bool IsActive { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }

        // -------------------------
        // Constructors
        // -------------------------
        private VehicleType() { } // للـ EF Core

        public VehicleType(
            string name,
            double maxDistance,
            double maxMergeExtra,
            double commission)
        {
            UpdateDetails(name, maxDistance, maxMergeExtra, commission);
            IsActive = true;
            CreatedAt = DateTimeOffset.UtcNow;
        }

        // -------------------------
        // Business Logic Actions
        // -------------------------

        public void UpdateDetails(
            string name,
            double maxDistance,
            double maxMergeExtra,
            double commission)
        {
            VehicleName = NormalizeRequired(name, 100, nameof(VehicleName));

            if (maxDistance <= 0)
                throw new ArgumentException("Max distance must be greater than 0.");

            if (maxMergeExtra < 0)
                throw new ArgumentException("Max merge extra distance cannot be negative.");

            if (commission < 0 || commission > 1.0)
                throw new ArgumentException("Commission must be between 0 and 1 (e.g., 0.15 for 15%).");

            MaxDistanceKm = maxDistance;
            MaxMergeExtraKm = maxMergeExtra;
            CommissionPercent = commission;
        }

        public void Deactivate()
        {
            IsActive = false;
            // ملاحظة: لا نمنع السائقين الحاليين هنا، بل المنطق في نظام الطلبات 
            // سيتحقق من IsActive عند إنشاء طلبات جديدة أو تسجيل سائقين جدد.
        }

        public void Activate() => IsActive = true;

        // -------------------------
        // Private Helpers
        // -------------------------
        private static string NormalizeRequired(string? value, int maxLen, string fieldName)
        {
            var v = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
            if (v is null) throw new ArgumentException($"{fieldName} is required.");
            if (v.Length > maxLen) throw new ArgumentException($"{fieldName} is too long.");
            return v;
        }
    }
}