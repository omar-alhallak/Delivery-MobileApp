using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.DomainExceptions;
using DeliveryApp.Domain.Entities.DriverRequest;
using DeliveryApp.Domain.Enums.DriverEnums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Domain.Entities.Drivers
{
    public class DriverRequest
    {
        
        public DriverRequestID ID { get; private set; } 
        public UserID UserId { get; private set; }     
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
        public UserID? ReviewedByAdminId { get; private set; } 
        public DateTimeOffset? ReviewedAt { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }

        private DriverRequest() { } 

        public DriverRequest(
            DriverRequestID id,
            UserID userId,
            VehicleTypeID vehicleTypeId,
            string fullName,
            string fatherName,
            string nationalIdNumber,
            string personalPhotoUrl,
            string nationalIdPhotoUrl,
            DateTimeOffset createdAtUtc)
        {
            ID = id;
            UserId = userId;
            VehicleTypeID = vehicleTypeId;
            CreatedAt = createdAtUtc;
            Status = DriverRequestStatus.Pending;

            // التحقق  للبيانات الأساسية
            UpdatePersonalInfo(fullName, fatherName, nationalIdNumber);
            UpdateBasePhotos(personalPhotoUrl, nationalIdPhotoUrl);
        }
       

        //للدراجة النارية او السيارة 
        public void SetVehicleDetails(
            string licensePhoto,
            string licenseNumber,
            string plateNumber)
        {
            // التحقق من البيانات
            if (string.IsNullOrWhiteSpace(licensePhoto)) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, field: nameof(licensePhoto));

            if (string.IsNullOrWhiteSpace(licenseNumber)) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, field: nameof(licenseNumber));

            if (string.IsNullOrWhiteSpace(plateNumber)) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, field: nameof(plateNumber));

            DrivingLicensePhotoUrl = licensePhoto.Trim();
            DrivingLicenseNumber = licenseNumber.Trim();
            VehiclePlateNumber = plateNumber.Trim();
        }

        // للدراجة الهوائية والكهربائية
        public void SetAsSimpleVehicle()
        {
            DrivingLicensePhotoUrl = null;
            DrivingLicenseNumber = null;
            VehiclePlateNumber = null;
        }

        // عمليات المراجعة 

        public void Approve(UserID adminId, DateTimeOffset reviewedAtUtc)
        {
            //لازم يكون الطلب معلق لينقبل
            if (Status != DriverRequestStatus.Pending)
                throw new InvalidOperationException("Only pending applications can be approved.");

            Status = DriverRequestStatus.Approved;
            ReviewedByAdminId = adminId;
            ReviewedAt = reviewedAtUtc;
        }

        public void Reject(UserID adminId, DateTimeOffset reviewedAtUtc)
        {
            if (Status != DriverRequestStatus.Pending)
                throw new InvalidOperationException("Only pending requests can be rejected.");
            Status = DriverRequestStatus.Rejected;
            ReviewedByAdminId = adminId;
            ReviewedAt = reviewedAtUtc;
            //مستقبلا ممكن نضيف حقل لسبب الرفض 
        }

        //  للتحقق

        private void UpdatePersonalInfo(string fullName, string fatherName, string nationalId)
        {
            if (string.IsNullOrWhiteSpace(fullName)) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, field: nameof(fullName));

            if (fullName.Length > 150) throw new DomainValidationException
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, field: nameof(fullName));

            FullName = fullName.Trim();
            FatherName = fatherName.Trim();
            NationalIdNumber = nationalId.Trim();
        }

        private void UpdateBasePhotos(string personalUrl, string nationalIdUrl)
        {
            PersonalPhotoUrl = personalUrl.Trim();
            NationalIdPhotoUrl = nationalIdUrl.Trim();
        }
    }
}