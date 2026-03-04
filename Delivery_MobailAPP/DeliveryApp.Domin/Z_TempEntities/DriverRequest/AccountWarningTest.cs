using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Domain.Entities.DriverRequest
{
    public enum WarningEntityType : byte
    {
        Customer = 1,
        Driver = 2,
        Merchant = 3
    }

    public enum WarningSeverity : byte
    {
        Low = 1,      // تنبيه بسيط
        Medium = 2,   // تحذير جدي
        High = 3,     // خطر (قد يؤدي لحظر تلقائي)
        Critical = 4  // حظر فوري
    }

    public class AccountWarningTest
    {
        public Guid WarningID { get; private set; }

        // الطرف المتلقي للتحذير (سائق/تاجر/عميل)
        public Guid EntityID { get; private set; }
        public WarningEntityType EntityType { get; private set; }

        // الطلب المرتبط بالمشكلة (اختياري)
        public Guid? RelatedOrderID { get; private set; }
        public Guid CreatedByAdminId { get; private set; }

        public string Reason { get; private set; } = default!;
        public WarningSeverity Severity { get; private set; }

        // القرار المتخذ (مثلاً: حظر لمدة 3 أيام، خصم من المحفظة، إلخ)
        public string Decision { get; private set; } = default!;

        public string? Notes { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; }
        public DateTimeOffset? ExpiresAt { get; private set; } // متى ينتهي مفعول التحذير أو الحظر
        public DateTimeOffset? DecidedAt { get; private set; }

        private AccountWarningTest() { } // للـ EF Core

        public AccountWarningTest(
            Guid entityId,
            WarningEntityType entityType,
            Guid adminId,
            string reason,
            WarningSeverity severity,
            string decision,
            Guid? relatedOrderId = null,
            DateTimeOffset? expiresAt = null)
        {
            WarningID = Guid.NewGuid();
            EntityID = entityId;
            EntityType = entityType;
            CreatedByAdminId = adminId;

            Reason = NormalizeRequired(reason, 500, nameof(Reason));
            Decision = NormalizeRequired(decision, 500, nameof(Decision));
            Severity = severity;

            RelatedOrderID = relatedOrderId;
            ExpiresAt = expiresAt;

            CreatedAt = DateTimeOffset.UtcNow;
            DecidedAt = DateTimeOffset.UtcNow;
        }

        // -------------------------
        // Business Actions
        // -------------------------

        public bool IsExpired => ExpiresAt.HasValue && ExpiresAt.Value < DateTimeOffset.UtcNow;

        public void UpdateDecision(string newDecision, WarningSeverity newSeverity)
        {
            Decision = NormalizeRequired(newDecision, 500, nameof(Decision));
            Severity = newSeverity;
            DecidedAt = DateTimeOffset.UtcNow;
        }

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
