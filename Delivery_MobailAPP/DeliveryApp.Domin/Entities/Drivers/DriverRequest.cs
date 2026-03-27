using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.DomainExceptions;
using DeliveryApp.Domain.Enums.DriverEnums;
using DeliveryApp.Domain.DomainErrors.DriverErrors;

namespace DeliveryApp.Domain.Entities.Drivers
{
    public class DriverRequest // يمثل طلب المستخدم ليصبح سائق
    {
        // -------------------------
        //            Key
        // -------------------------

        public DriverRequestID ID { get; private set; } // PK معرف طلب الانضمام كسائق

        // -------------------------
        //         Relations
        // -------------------------

        public UserID UserID { get; private set; } // المستخدم الي قدم الطلب
        public VehicleTypeID VehicleTypeID { get; private set; } // نوع المركبة

        // -------------------------
        //      Personal Info
        // -------------------------

        public string FullName { get; private set; } = null!; // الاسم الكامل
        public string FatherName { get; private set; } = null!; // اسم الأب
        public string NationalIdNumber { get; private set; } = null!; // رقم الهوية الوطنية

        // -------------------------
        //          Photos
        // -------------------------

        public string PersonalPhotoUrl { get; private set; } = null!; // صورة شخصية
        public string NationalIdPhotoUrl { get; private set; } = null!; // صورة الهوية الوطنية

        // -------------------------
        //     Vehicle Details
        // -------------------------

        public string? DrivingLicensePhotoUrl { get; private set; } // صورة رخصة القيادة
        public string? DrivingLicenseNumber { get; private set; } // رقم رخصة القيادة
        public string? VehiclePlateNumber { get; private set; } // رقم لوحة المركبة

        // -------------------------
        //          Review
        // -------------------------

        public DriverRequestStatus Status { get; private set; } // حالة الطلب
        public UserID? ReviewedByAdminID { get; private set; } // المشرف الي راجع الطلب
        public DateTimeOffset? ReviewedAt { get; private set; } // وقت المراجعة
        public string? RejectionReason { get; private set; } // سبب الرفض 

        // -------------------------
        //           Dates
        // -------------------------

        public DateTimeOffset CreatedAt { get; private set; } // وقت إنشاء الطلب

        private DriverRequest() { }

        public DriverRequest(DriverRequestID id, UserID userId, VehicleTypeID vehicleTypeId, string fullName, string fatherName,
            string nationalIdNumber, string personalPhotoUrl, string nationalIdPhotoUrl, DateTimeOffset createdAtUtc)
        {
            if (id.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(id));

            if (userId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(userId));

            if (vehicleTypeId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(vehicleTypeId));

            if (createdAtUtc == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(createdAtUtc));

            ID = id;
            UserID = userId;
            VehicleTypeID = vehicleTypeId;

            CreatedAt = createdAtUtc;
            Status = DriverRequestStatus.Pending;

            SetPersonalInfo(fullName, fatherName, nationalIdNumber);
            SetBasePhotos(personalPhotoUrl, nationalIdPhotoUrl);
        }

        // ------------------------
        //     Vehicle details
        // ------------------------

        public void SetVehicleLicense(string licensePhotoUrl, string licenseNumber, string plateNumber) // إدخال بيانات الرخصة ولوحة المركبة
        {
            EnsurePending();

            DrivingLicensePhotoUrl = NormalizeRequired(licensePhotoUrl, nameof(DrivingLicensePhotoUrl), maxLen: 500);
            DrivingLicenseNumber = NormalizeRequired(licenseNumber, nameof(DrivingLicenseNumber), maxLen: 50);
            VehiclePlateNumber = NormalizeRequired(plateNumber, nameof(VehiclePlateNumber), maxLen: 30);
        }

        public void SetVehicleWithoutLicense() // للمركبات التي لا تحتاج رخصة
        {
            EnsurePending();

            DrivingLicensePhotoUrl = null;
            DrivingLicenseNumber = null;
            VehiclePlateNumber = null;
        }

        // -------------------------
        //          Review
        // -------------------------

        public void Approve(UserID adminId, DateTimeOffset reviewedAtUtc, bool requiresVehicleDetails) // قبول الطلب
        {
            EnsurePending();

            if (adminId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(adminId));

            if (reviewedAtUtc == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(reviewedAtUtc));

            if (reviewedAtUtc < CreatedAt) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(reviewedAtUtc));

            if (requiresVehicleDetails && !HasVehicleDetails()) throw new DomainRuleViolationException
                    (DriverRequestErrors.VehicleDetailsRequiredCode, DriverRequestErrors.VehicleDetailsRequiredMessage);

            Status = DriverRequestStatus.Approved;
            ReviewedByAdminID = adminId;
            ReviewedAt = reviewedAtUtc;
            RejectionReason = null;
        }

        public void Reject(UserID adminId, DateTimeOffset reviewedAtUtc, string rejectionReason) // رفض الطلب
        {
            EnsurePending();

            if (adminId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(adminId));

            if (reviewedAtUtc == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(reviewedAtUtc));

            if (reviewedAtUtc < CreatedAt) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(reviewedAtUtc));

            RejectionReason = NormalizeRequired(rejectionReason, nameof(RejectionReason), maxLen: 500);

            Status = DriverRequestStatus.Rejected;
            ReviewedByAdminID = adminId;
            ReviewedAt = reviewedAtUtc;
        }

        // -------------------------
        //          setters
        // -------------------------

        private void SetPersonalInfo(string fullName, string fatherName, string nationalIdNumber) // تعبئة البيانات الشخصية الأساسية
        {
            EnsurePending();

            FullName = NormalizeRequired(fullName, nameof(FullName), maxLen: 150);
            FatherName = NormalizeRequired(fatherName, nameof(FatherName), maxLen: 150);
            NationalIdNumber = NormalizeRequired(nationalIdNumber, nameof(NationalIdNumber), maxLen: 50);
        }

        private void SetBasePhotos(string personalPhotoUrl, string nationalIdPhotoUrl) // تعبئة الصور الأساسية المطلوبة للطلب
        {
            EnsurePending();

            PersonalPhotoUrl = NormalizeRequired(personalPhotoUrl, nameof(PersonalPhotoUrl), maxLen: 500);
            NationalIdPhotoUrl = NormalizeRequired(nationalIdPhotoUrl, nameof(NationalIdPhotoUrl), maxLen: 500);
        }

        // -------------------------
        //          Helpers
        // -------------------------

        private void EnsurePending() // التأكد أن الطلب ما زال قيد المراجعة
        {
            if (Status != DriverRequestStatus.Pending) throw new DomainRuleViolationException
                    (DriverRequestErrors.NotPendingCode, DriverRequestErrors.NotPendingMessage);
        }

        private bool HasVehicleDetails() // التحقق من وجود جميع بيانات المركبة المطلوبة
        {
            return !string.IsNullOrWhiteSpace(DrivingLicensePhotoUrl)
                && !string.IsNullOrWhiteSpace(DrivingLicenseNumber)
                && !string.IsNullOrWhiteSpace(VehiclePlateNumber);
        }

        private static string NormalizeRequired(string? value, string field, int maxLen) // تنظيف النص والتحقق من طوله
        {
            if (string.IsNullOrWhiteSpace(value)) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, field);

            var v = value.Trim();

            if (v.Length > maxLen) throw new DomainValidationException
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, field);

            return v;
        }
    }
}