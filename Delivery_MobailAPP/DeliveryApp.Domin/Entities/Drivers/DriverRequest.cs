using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.DomainErrors.Drivers;
using DeliveryApp.Domain.DomainExceptions;
using DeliveryApp.Domain.Enums.DriverEnums;

namespace DeliveryApp.Domain.Entities.Drivers
{
    public class DriverRequest
    {
        public DriverRequestID ID { get; private set; }
        public UserID UserID { get; private set; }
        public VehicleTypeID VehicleTypeID { get; private set; }

        public string FullName { get; private set; } = null!;
        public string FatherName { get; private set; } = null!;
        public string NationalIdNumber { get; private set; } = null!;

        public string PersonalPhotoUrl { get; private set; } = null!;
        public string NationalIdPhotoUrl { get; private set; } = null!;

        public string? DrivingLicensePhotoUrl { get; private set; }
        public string? DrivingLicenseNumber { get; private set; }
        public string? VehiclePlateNumber { get; private set; }

        public DriverRequestStatus Status { get; private set; }

        public UserID? ReviewedByAdminID { get; private set; }
        public DateTimeOffset? ReviewedAt { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; }

        private DriverRequest() { }

        public DriverRequest(DriverRequestID id, UserID UserId, VehicleTypeID VehicleTypeId, string fullName,
            string fatherName, string nationalIdNumber, string personalPhotoUrl, string nationalIdPhotoUrl, DateTimeOffset CreatedAtUtc)
        {
            if (id.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(ID));

            if (UserId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(UserID));

            if (VehicleTypeId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(VehicleTypeID));

            ID = id;
            UserID = UserId;
            VehicleTypeID = VehicleTypeId;

            CreatedAt = CreatedAtUtc;
            Status = DriverRequestStatus.Pending;

            SetPersonalInfo(fullName, fatherName, nationalIdNumber);
            SetBasePhotos(personalPhotoUrl, nationalIdPhotoUrl);
        }

        // ------------------------
        //     Vehicle details
        // ------------------------

        public void SetVehicleLicense(string licensePhotoUrl, string licenseNumber, string plateNumber)
        {
            CheckISPending();

            DrivingLicensePhotoUrl = NormalizeRequired(licensePhotoUrl, nameof(DrivingLicensePhotoUrl), maxLen: 500);
            DrivingLicenseNumber = NormalizeRequired(licenseNumber, nameof(DrivingLicenseNumber), maxLen: 50);
            VehiclePlateNumber = NormalizeRequired(plateNumber, nameof(VehiclePlateNumber), maxLen: 30);
        }

        public void SetVehicleWithoutLicense()
        {
            CheckISPending();

            DrivingLicensePhotoUrl = null;
            DrivingLicenseNumber = null;
            VehiclePlateNumber = null;
        }

        // -------------------------
        //          Review
        // -------------------------

        public void Approve(UserID AdminId, DateTimeOffset ReviewedAtUtc, bool requiresVehicleDetails)
        {
            CheckISPending();

            if (AdminId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(ReviewedByAdminID));

            if (ReviewedAtUtc < CreatedAt) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(ReviewedAt));

            if (requiresVehicleDetails && !HasVehicleDetails()) throw new DomainRuleViolationException
                    (DriverRequestErrors.VehicleDetailsRequiredCode, DriverRequestErrors.VehicleDetailsRequiredMessage);

            Status = DriverRequestStatus.Approved;
            ReviewedByAdminID = AdminId;
            ReviewedAt = ReviewedAtUtc;
        }

        public void Reject(UserID AdminId, DateTimeOffset ReviewedAtUtc)
        {
            CheckISPending();

            if (AdminId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(ReviewedByAdminID));

            if (ReviewedAtUtc < CreatedAt) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(ReviewedAt));

            Status = DriverRequestStatus.Rejected;
            ReviewedByAdminID = AdminId;
            ReviewedAt = ReviewedAtUtc;
        }

        // -------------------------
        //      Private setters
        // -------------------------

        private void SetPersonalInfo(string fullName, string fatherName, string nationalIdNumber)
        {
            CheckISPending();

            FullName = NormalizeRequired(fullName, nameof(FullName), maxLen: 150);
            FatherName = NormalizeRequired(fatherName, nameof(FatherName), maxLen: 150);
            NationalIdNumber = NormalizeRequired(nationalIdNumber, nameof(NationalIdNumber), maxLen: 50);
        }

        private void SetBasePhotos(string personalPhotoUrl, string nationalIdPhotoUrl)
        {
            CheckISPending();

            PersonalPhotoUrl = NormalizeRequired(personalPhotoUrl, nameof(PersonalPhotoUrl), maxLen: 500);
            NationalIdPhotoUrl = NormalizeRequired(nationalIdPhotoUrl, nameof(NationalIdPhotoUrl), maxLen: 500);
        }

        // -------------------------
        //          Helpers
        // -------------------------

        private void CheckISPending()
        {
            if (Status != DriverRequestStatus.Pending) throw new DomainRuleViolationException
                    (DriverRequestErrors.NotPendingCode, DriverRequestErrors.NotPendingMessage);
        }

        private bool HasVehicleDetails() => !string.IsNullOrWhiteSpace(DrivingLicensePhotoUrl)
               && !string.IsNullOrWhiteSpace(DrivingLicenseNumber) && !string.IsNullOrWhiteSpace(VehiclePlateNumber);

        private static string NormalizeRequired(string? value, string field, int maxLen)
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