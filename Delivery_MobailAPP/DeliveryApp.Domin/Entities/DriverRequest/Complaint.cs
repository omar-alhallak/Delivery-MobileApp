using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Domain.Entities.DriverRequest
{
    public enum ComplaintStatus : byte
    {
        Pending = 1,
        UnderReview = 2,
        Resolved = 3
    }

    public enum ComplaintTargetType : byte
    {
        Merchant = 1,
        Driver = 2,
        Customer = 3
    }

    public class Complaint
    {
        public Guid ComplaintID { get; private set; }
        public Guid CreatedByUserID { get; private set; }
        public Guid OrderID { get; private set; }

        // الطرف المشتكى عليه
        public Guid TargetID { get; private set; }
        public ComplaintTargetType TargetType { get; private set; }

        public string Reason { get; private set; } = default!;
        public string Message { get; private set; } = default!;

        public ComplaintStatus Status { get; private set; }

        // بيانات الإدارة
        public Guid? ReviewedByAdminId { get; private set; }
        public string? AdminResponse { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; }
        public DateTimeOffset? ResolvedAt { get; private set; }

        private Complaint() { } // للـ EF Core

        public Complaint(
            Guid createdByUserId,
            Guid orderId,
            Guid targetId,
            ComplaintTargetType targetType,
            string reason,
            string message)
        {
            CreatedByUserID = createdByUserId;
            OrderID = orderId;
            TargetID = targetId;
            TargetType = targetType;

            Reason = NormalizeRequired(reason, 200, nameof(Reason));
            Message = NormalizeRequired(message, 2000, nameof(Message));

            Status = ComplaintStatus.Pending;
            CreatedAt = DateTimeOffset.UtcNow;
        }

        // -------------------------
        // Business Actions (أفعال البيزنس)
        // -------------------------

        public void AssignToAdmin(Guid adminId)
        {
            if (Status == ComplaintStatus.Resolved)
                throw new InvalidOperationException("Cannot review a resolved complaint.");

            ReviewedByAdminId = adminId;
            Status = ComplaintStatus.UnderReview;
        }

        public void Resolve(string response)
        {
            if (ReviewedByAdminId == null)
                throw new InvalidOperationException("Complaint must be assigned to an admin before resolving.");

            AdminResponse = NormalizeRequired(response, 2000, nameof(AdminResponse));
            Status = ComplaintStatus.Resolved;
            ResolvedAt = DateTimeOffset.UtcNow;
        }

        private static string NormalizeRequired(string? value, int maxLen, string fieldName)
        {
            var v = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
            if (v is null) throw new ArgumentException($"{fieldName} is required.");
            if (v.Length > maxLen) throw new ArgumentException($"{fieldName} is too long.");
            return v;
        }
    }
}
