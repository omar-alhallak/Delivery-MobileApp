using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.ValueObjects;
using DeliveryApp.Domain.DomainExceptions;
using DeliveryApp.Domain.Enums.EngagementEnums;
using DeliveryApp.Domain.DomainErrors.DriverErrors;

namespace DeliveryApp.Domain.Entities.Drivers
{
    public class Driver // يمثل سائق
    {
        // -------------------------
        //            Key
        // -------------------------

        public UserID ID { get; private set; } // PK سائق هو يوزر

        // -------------------------
        //          Vehicle
        // -------------------------

        public VehicleTypeID VehicleTypeID { get; private set; } // نوع المركبة

        // -------------------------
        //      Admin Control
        // -------------------------

        public bool IsEnabled { get; private set; } = true; // هل السائق مفعل أو معطل
        public UserID? DisabledByAdminID { get; private set; } // إذا معطل أي مشرف قام بتعطيله
        public DateTimeOffset? DisabledAt { get; private set; } // متى تم تعطيل الحساب

        // -------------------------
        //       Availability
        // -------------------------

        public bool IsAvailable { get; private set; } // هل سائق متاح لاستقبال الطبيات
        public int ActiveOrdersCount { get; private set; } // كم طلب نشط عند سائق

        // -------------------------
        //          Active
        // -------------------------

        public DateTimeOffset? LastSeenAt { get; private set; } // أخر وقت كان السائق نشط فيه

        // -------------------------
        //         Location
        // -------------------------

        public GeoPoint? CurrentLocation { get; private set; } // أخر موقع للسائق
        public DateTimeOffset? LastLocationAt { get; private set; } // متى تم تحديث هذا الموقع

        // -------------------------
        //          Rating
        // -------------------------

        public decimal AverageRating { get; private set; } // متوسط تقييم السائق
        public int RatingsCount { get; private set; } // عدد تقيمات

        // -------------------------
        //         Approval
        // -------------------------

        public UserID ApprovedByAdminID { get; private set; } // من المشرف الذي وافق على عمل سائق
        public DateTimeOffset ApprovedAt { get; private set; } // وقت الموافقة

        // -------------------------
        //          Dates
        // -------------------------

        public DateTimeOffset CreatedAt { get; private set; } // وقت إنشاء الحساب

        private Driver() { }

        public Driver(UserID userId, VehicleTypeID vehicleTypeId, UserID approvedByAdminId, DateTimeOffset approvedAtUtc)
        {
            if (userId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(userId));

            if (vehicleTypeId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(vehicleTypeId));

            if (approvedByAdminId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(approvedByAdminId));

            if (approvedAtUtc == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(approvedAtUtc));

            ID = userId;
            VehicleTypeID = vehicleTypeId;

            ApprovedByAdminID = approvedByAdminId;
            ApprovedAt = approvedAtUtc;
            CreatedAt = approvedAtUtc;

            AverageRating = 0;
            RatingsCount = 0;

            IsAvailable = false;
            ActiveOrdersCount = 0;
            IsEnabled = true;
        }

        // -------------------------------
        //      disable/enable driver
        // -------------------------------

        public void Disable(UserID adminId, DateTimeOffset utcNow) // تعطيل السائق من قبل المشرف
        {
            if (adminId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(adminId));

            if (utcNow == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(utcNow));

            if (!IsEnabled) return;

            IsEnabled = false;
            IsAvailable = false;

            DisabledByAdminID = adminId;
            DisabledAt = utcNow;
        }

        public void Enable(UserID adminId) // إعادة تفعيل السائق بعد تعطيله
        {
            if (adminId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(adminId));

            if (IsEnabled) return;

            IsEnabled = true;
            DisabledByAdminID = null;
            DisabledAt = null;
        }

        private void EnsureEnabled() // التأكد أن السائق غير معطل قبل أي عملية تشغيلية
        {
            if (!IsEnabled) throw new DomainRuleViolationException
                    (DriverErrors.DisabledCode, DriverErrors.DisabledMessage);
        }

        // ---------- Online ----------

        public bool IsOnline(DateTimeOffset utcNow, TimeSpan threshold) // التحقق هل السائق متصل حسب آخر نشاط
        {
            if (utcNow == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(utcNow));

            if (threshold <= TimeSpan.Zero) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(threshold));

            return LastSeenAt.HasValue && (utcNow - LastSeenAt.Value) <= threshold;
        }

        // -------------------------
        //   Activity -- Location
        // -------------------------

        public void Touch(DateTimeOffset utcNow) // تحديث آخر ظهور للسائق بدون تغيير الموقع
        {
            EnsureEnabled();

            if (utcNow == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(utcNow));

            LastSeenAt = utcNow;
        }

        public void UpdateLocation(decimal lat, decimal lng, DateTimeOffset utcNow) // تحديث موقع السائق وآخر ظهور له
        {
            EnsureEnabled();

            if (utcNow == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(utcNow));

            CurrentLocation = GeoPoint.Create(lat, lng);
            LastLocationAt = utcNow;
            LastSeenAt = utcNow;
        }

        // ---------- Availability ----------

        public void SetAvailable(bool available, DateTimeOffset utcNow, TimeSpan onlineThreshold) // تغيير حالة التوفر
        {
            EnsureEnabled();

            if (utcNow == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(utcNow));

            if (onlineThreshold <= TimeSpan.Zero) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(onlineThreshold));

            if (available)
            {
                if (!IsOnline(utcNow, onlineThreshold)) throw new DomainRuleViolationException
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

        public void ChangeVehicle(VehicleTypeID newVehicleTypeId) // تغيير نوع المركبة المستخدمة من السائق
        {
            EnsureEnabled();

            if (newVehicleTypeId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(newVehicleTypeId));

            VehicleTypeID = newVehicleTypeId;
        }

        // -------------------------
        //      Orders counter
        // -------------------------

        public void AssignOrder() // زيادة عدد الطلبات النشطة عند إرسال طلب جديد للسائق
        {
            EnsureEnabled();
            ActiveOrdersCount++;
        }

        public void CompleteOrder() // إنقاص عدد الطلبات النشطة عند إنتهاء الطلب
        {
            EnsureEnabled();

            if (ActiveOrdersCount <= 0) return;
            ActiveOrdersCount--;
        }

        // -------------------------
        //         Rating
        // -------------------------

        public void AddRating(RatingStars stars) // إضافة تقييم جديد
        {
            ValidateRatingStars(stars);

            var value = (int)stars;

            AverageRating = ((AverageRating * RatingsCount) + value) / (RatingsCount + 1);
            RatingsCount++;
        }

        public void UpdateRating(RatingStars oldStars, RatingStars newStars) // تحديث تقييم
        {
            ValidateRatingStars(oldStars);
            ValidateRatingStars(newStars);

            if (RatingsCount <= 0) throw new DomainRuleViolationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage);

            var oldValue = (int)oldStars;
            var newValue = (int)newStars;

            AverageRating = ((AverageRating * RatingsCount) - oldValue + newValue) / RatingsCount;
        }

        private static void ValidateRatingStars(RatingStars stars) // تحقق من صحة التقييم
        {
            if (!Enum.IsDefined(typeof(RatingStars), stars)) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(stars));
        }
    }
}