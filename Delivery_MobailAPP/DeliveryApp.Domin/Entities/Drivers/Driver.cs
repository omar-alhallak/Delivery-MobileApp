using DeliveryApp.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Domain.Entities.Drivers
{

    public class Driver
    {
        // 1. المعرفات (الأعمدة الأساسية)
        public DriverID ID { get; private set; } // PK + FK بنفس الوقت
        public PublicCode? PublicDriverID { get; private set; } // الكود الجميل (DRV-000001)

        public VehicleTypeID VehicleTypeID { get; private set; } // نوع المركبة (StrongID)

        // 2. حالات السائق
        public bool IsApproved { get; private set; }
        public bool IsAvailable { get; private set; }
        public DateTimeOffset? LastSeenAt { get; private set; }

        // 3. الموقع الجغرافي (استخدام decimal كما طلبت)
        public decimal? CurrentLat { get; private set; }
        public decimal? CurrentLng { get; private set; }
        public DateTimeOffset? LastLocationAt { get; private set; }

        // 4. العمليات الحالية والإحصائيات
        public int ActiveOrdersCount { get; private set; }
        public Guid? ApprovedByAdminId { get; private set; }
        public DateTimeOffset? ApprovedAt { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }

        // المشيد الخاص بالـ EF Core
        private Driver() { }

        // المشيد الأساسي (Constructor)
        public Driver(DriverID userId, VehicleTypeID vehicleTypeId, DateTimeOffset createdAtUtc)
        {
            // هنا نستخدم خاصية IsEmpty الموجودة داخل الـ StrongID تبعك
            if (userId.IsEmpty) throw new ArgumentException("UserId cannot be empty.");
            if (vehicleTypeId.IsEmpty) throw new ArgumentException("VehicleTypeId cannot be empty.");

            ID = userId;
            VehicleTypeID = vehicleTypeId;

            IsApproved = false;
            IsAvailable = false;
            ActiveOrdersCount = 0;
            CreatedAt = createdAtUtc;
        }

        // -----------------------------------------
        // العمليات (Methods)
        // -----------------------------------------

        // تعيين الكود العام (PublicCode)
        public void SetPublicId(PublicCode code)
        {
            // بما أن النوع PublicCode، فنحن ضامنون أنه محقق (Validated) سلفاً
            PublicDriverID = code;
        }

        public void Approve(Guid adminId, DateTimeOffset utcNow)
        {
            if (adminId == Guid.Empty) throw new ArgumentException("AdminId cannot be empty.");

            IsApproved = true;
            ApprovedByAdminId = adminId;
            ApprovedAt = utcNow;
        }

        public void UpdateLocation(decimal lat, decimal lng, DateTimeOffset utcNow)
        {
            // التحقق من صحة الإحداثيات منطقياً
            if (lat < -90 || lat > 90) throw new ArgumentOutOfRangeException(nameof(lat));
            if (lng < -180 || lng > 180) throw new ArgumentOutOfRangeException(nameof(lng));

            CurrentLat = lat;
            CurrentLng = lng;
            LastLocationAt = utcNow;
            LastSeenAt = utcNow; // السائق أرسل موقعه، إذن هو "نشط"
        }

        public void SetAvailable(bool available, DateTimeOffset utcNow)
        {
            if (available)
            {
                if (!IsApproved) throw new InvalidOperationException("Driver not approved.");

                // التأكد أنه "أونلاين" (آخر ظهور أقل من 5 دقائق مثلاً)
                var isOnline = LastSeenAt.HasValue && (utcNow - LastSeenAt.Value).TotalMinutes <= 5;
                if (!isOnline) throw new InvalidOperationException("Driver is offline.");
            }
            IsAvailable = available;
        }

        public void ChangeVehicle(VehicleTypeID newVehicleTypeId)
        {
            if (newVehicleTypeId.IsEmpty) throw new ArgumentException("New vehicle type is required.");
            VehicleTypeID = newVehicleTypeId;
        }

        public void IncrementOrders() => ActiveOrdersCount++;
        public void DecrementOrders() => ActiveOrdersCount = Math.Max(0, ActiveOrdersCount - 1);
    }
}