using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Domain.Entities.DriverRequest
{
    public class DriverRequest
    {
       

            public Guid Id { get; private set; }
            // صاحب الطلب
            public Guid UserId { get; private set; }

            // بيانات حساسة
            public string FullName { get; private set; } = null!;
            public string FatherName { get; private set; } = null!;
            public string NationalIdNumber { get; private set; } = null!;

            // صور
            public string PersonalPhotoUrl { get; private set; } = null!;
            public string NationalIdPhotoUrl { get; private set; } = null!;

            // نوع المركبة
            public Guid VehicleTypeId { get; private set; }

            // للمتور فقط
            public string? DrivingLicensePhotoUrl { get; private set; }
            public string? DrivingLicenseNumber { get; private set; }
            public string? VehiclePlateNumber { get; private set; }

            public DriverApplicationStatus Status { get; private set; } = DriverApplicationStatus.Pending;

            // مراجعة المشرف
            public Guid? ReviewedByAdminId { get; private set; }
            public DateTimeOffset? ReviewedAt { get; private set; }

            public DateTimeOffset CreatedAt { get; private set; }

            private DriverRequest() { }

            public DriverRequest(
                Guid id,
                Guid userId,
                string fullName,
                string fatherName,
                string nationalIdNumber,
                string personalPhotoUrl,
                string nationalIdPhotoUrl,
                Guid vehicleTypeId,
                DateTimeOffset createdAtUtc,
                bool isMotorcycle, // نمررها من الـ Application لأن VehicleType اسم/نوع غالباً برا الدومين هون
                string? drivingLicensePhotoUrl = null,
                string? drivingLicenseNumber = null,
                string? vehiclePlateNumber = null
            )
            {
                if (id == Guid.Empty) throw new ArgumentException("Id cannot be empty.");
                if (userId == Guid.Empty) throw new ArgumentException("UserId cannot be empty.");
                if (vehicleTypeId == Guid.Empty) throw new ArgumentException("VehicleTypeId cannot be empty.");

                Id = id;
                UserId = userId;
                CreatedAt = createdAtUtc;

                SetIdentity(fullName, fatherName, nationalIdNumber);
                SetPhotos(personalPhotoUrl, nationalIdPhotoUrl);

                VehicleTypeId = vehicleTypeId;

                SetMotorcycleDetails(
                    isMotorcycle,
                    drivingLicensePhotoUrl,
                    drivingLicenseNumber,
                    vehiclePlateNumber
                );

                Status = DriverApplicationStatus.Pending;
            }

            // -------------------------
            // Updates before review
            // -------------------------

            public void UpdateIdentity(string fullName, string fatherName, string nationalIdNumber)
            {
                EnsurePending();
                SetIdentity(fullName, fatherName, nationalIdNumber);
            }

            public void UpdatePhotos(string personalPhotoUrl, string nationalIdPhotoUrl)
            {
                EnsurePending();
                SetPhotos(personalPhotoUrl, nationalIdPhotoUrl);
            }

            public void ChangeVehicleType(
                Guid vehicleTypeId,
                bool isMotorcycle,
                string? drivingLicensePhotoUrl = null,
                string? drivingLicenseNumber = null,
                string? vehiclePlateNumber = null
            )
            {
                EnsurePending();

                if (vehicleTypeId == Guid.Empty) throw new ArgumentException("VehicleTypeId cannot be empty.");
                VehicleTypeId = vehicleTypeId;

                SetMotorcycleDetails(isMotorcycle, drivingLicensePhotoUrl, drivingLicenseNumber, vehiclePlateNumber);
            }

            // -------------------------
            // Review actions
            // -------------------------

            public void Approve(Guid adminId, DateTimeOffset reviewedAtUtc)
            {
                EnsurePending();

                if (adminId == Guid.Empty) throw new ArgumentException("AdminId cannot be empty.");

                Status = DriverApplicationStatus.Approved;
                ReviewedByAdminId = adminId;
                ReviewedAt = reviewedAtUtc;
            }

            public void Reject(Guid adminId, DateTimeOffset reviewedAtUtc)
            {
                EnsurePending();

                if (adminId == Guid.Empty) throw new ArgumentException("AdminId cannot be empty.");

                Status = DriverApplicationStatus.Rejected;
                ReviewedByAdminId = adminId;
                ReviewedAt = reviewedAtUtc;
            }

            // -------------------------
            // Private helpers
            // -------------------------

            private void SetIdentity(string fullName, string fatherName, string nationalIdNumber)
            {
                FullName = NormalizeRequired(fullName, 150, "FullName");
                FatherName = NormalizeRequired(fatherName, 150, "FatherName");

                // الرقم الوطني: بما أنه Unique وعادة يكون رقم/نص ثابت
                NationalIdNumber = NormalizeRequired(nationalIdNumber, 50, "NationalIdNumber");
            }

            private void SetPhotos(string personalPhotoUrl, string nationalIdPhotoUrl)
            {
                PersonalPhotoUrl = NormalizeRequired(personalPhotoUrl, 500, "PersonalPhotoUrl");
                NationalIdPhotoUrl = NormalizeRequired(nationalIdPhotoUrl, 500, "NationalIdPhotoUrl");
            }

            private void SetMotorcycleDetails(
                bool isMotorcycle,
                string? drivingLicensePhotoUrl,
                string? drivingLicenseNumber,
                string? vehiclePlateNumber
            )
            {
                if (isMotorcycle)
                {
                    DrivingLicensePhotoUrl = NormalizeRequired(drivingLicensePhotoUrl, 500, "DrivingLicensePhotoUrl");
                    DrivingLicenseNumber = NormalizeRequired(drivingLicenseNumber, 100, "DrivingLicenseNumber");
                    VehiclePlateNumber = NormalizeRequired(vehiclePlateNumber, 50, "VehiclePlateNumber");
                }
                else
                {
                    // ممنوع يكونوا معبّيين إذا مو متور
                    DrivingLicensePhotoUrl = null;
                    DrivingLicenseNumber = null;
                    VehiclePlateNumber = null;
                }
            }

            private void EnsurePending()
            {
                if (Status != DriverApplicationStatus.Pending)
                    throw new InvalidOperationException("Only pending applications can be modified or reviewed.");
            }

            private static string NormalizeRequired(string? value, int maxLen, string fieldName)
            {
                var v = Normalize(value);
                if (v is null) throw new ArgumentException($"{fieldName} is required.");
                if (v.Length > maxLen) throw new ArgumentException($"{fieldName} is too long.");
                return v;
            }

            private static string? Normalize(string? s)=> string.IsNullOrWhiteSpace(s) ? null : s.Trim();
        }
    }


