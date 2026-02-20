using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Domain.Entities
{
    public class Driver
    {
        // PK + FK -> Users
        public Guid UserID { get; private set; }
        public string? PublicDriverID { get; private set; }

        public Guid VehicleTypeID { get; private set; }

        public bool IsApproved { get; private set; }
        public DateTimeOffset? LastSeenAt { get; private set; }
        public bool IsAvailable { get; private set; }

        public decimal? CurrentLat { get; private set; }
        public decimal? CurrentLng { get; private set; }
        public DateTimeOffset? LastLocationAt { get; private set; }

        public int ActiveOrdersCount { get; private set; }

        public Guid? ApprovedByAdminId { get; private set; } 

        public DateTimeOffset? ApprovedAt { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }

        private Driver() { }

        public Driver(Guid UserId, Guid VehicleTypeId, DateTimeOffset CreatedAtUtc)
        {
            if (UserId == Guid.Empty) throw new ArgumentException("UserId cannot be empty.");
            if (VehicleTypeId == Guid.Empty) throw new ArgumentException("VehicleTypeId cannot be empty.");

            UserID = UserId;
            VehicleTypeID = VehicleTypeId;

            IsApproved = false;
            IsAvailable = false;
            ActiveOrdersCount = 0;

            CreatedAt = CreatedAtUtc;
        }

        // -------------------------
        //         Public ID 
        // -------------------------
        public void SetPublicDriverId(string PublicId)
        {
            PublicId = Normalize(PublicId) ?? throw new ArgumentException("PublicDriverId cannot be empty.");
            if (PublicId.Length > 30) throw new ArgumentException("PublicDriverId is too long.");
            PublicDriverID = PublicId;
        }

        // -------------------------
        //         Approved
        // -------------------------
        public void Approve(Guid adminId, DateTimeOffset utcNow) // قبول السائق
        {
            if (adminId == Guid.Empty) throw new ArgumentException("AdminId cannot be empty.");

            IsApproved = true;
            ApprovedByAdminId = adminId;
            ApprovedAt = utcNow;
        }

        public void RejectApproval() // رفض السائق
        {
            IsApproved = false;
            ApprovedByAdminId = null;
            ApprovedAt = null;

            IsAvailable = false;
        }

        // تغيير المركبة
        public void ChangeVehicleType(Guid VehicleTypeId) 
        {
            if (VehicleTypeId == Guid.Empty) throw new ArgumentException("VehicleTypeId cannot be empty.");
            VehicleTypeID = VehicleTypeId;
        }

        // ------------------------------------------
        //   After (5 minutes) = LastSeenAt = false;
        // ------------------------------------------
        public void Heartbeat(DateTimeOffset utcNow) // إرسال نبضة كل فترة لتأكد أنه أونلاين
        {
            LastSeenAt = utcNow;
        }

        public bool IsOnline(DateTimeOffset utcNow, int OnlineWindowMinutes = 5) // بعد 5 دقائق من إغلاق التطبيق يصبح فولس
        {
            if (!LastSeenAt.HasValue) return false;
            return utcNow - LastSeenAt.Value <= TimeSpan.FromMinutes(OnlineWindowMinutes);
        }

        // قواعد الإتاحة
        public void SetAvailable(bool isAvailable, DateTimeOffset utcNow) 
        {
            if (isAvailable && !IsApproved)
                throw new InvalidOperationException("Driver must be approved before becoming available.");

            if (isAvailable && !IsOnline(utcNow))
                throw new InvalidOperationException("Driver must be online to become available.");

            IsAvailable = isAvailable;
        }

        // لتحقق من صحة الإحداثيات وتحديثها
        public void UpdateCurrentLocation(decimal lat, decimal lng, DateTimeOffset utcNow) 
        {
            if (lat < -90 || lat > 90) throw new ArgumentOutOfRangeException(nameof(lat), "Latitude out of range.");
            if (lng < -180 || lng > 180) throw new ArgumentOutOfRangeException(nameof(lng), "Longitude out of range.");

            CurrentLat = lat;
            CurrentLng = lng;
            LastLocationAt = utcNow;
            LastSeenAt = utcNow;
        }

        // -------------------------
        //       Active Order
        // -------------------------
        public void AddActiveOrder() 
        {
            ActiveOrdersCount++;
        }

        public void RemoveActiveOrder()
        {
            if (ActiveOrdersCount == 0) return;
            ActiveOrdersCount--;
        }

        private static string? Normalize(string? s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim(); // تنظيف النص قبل التخزين
    }
}