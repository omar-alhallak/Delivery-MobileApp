using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Domain.Entities.DriverRequest
{
    public enum NotificationType : byte
    {
        OrderUpdate = 1,      // تحديثات حالة الطلب
        ComplaintResponse = 2, // رد من الإدارة على شكوى
        AccountWarning = 3,   // تحذير بخصوص الحساب
        GeneralMarketing = 4, // عروض أو رسائل عامة
        SystemAlert = 5       // تنبيهات تقنية
    }

    public class Notification
    {
        public Guid NotificationID { get; private set; }
        public Guid UserId { get; private set; }

        // الكيان المرتبط بالتنبيه (اختياري لأن بعض التنبيهات قد تكون عامة)
        public Guid? RelatedEntityID { get; private set; }
        public NotificationType Type { get; private set; }

        public string Title { get; private set; } = default!;
        public string Body { get; private set; } = default!;
        public bool IsRead { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }

        private Notification() { } // للـ EF Core

        public Notification(
            Guid userId,
            string title,
            string body,
            NotificationType type,
            Guid? relatedEntityId = null)
        {
            UserId = userId;
            Title = NormalizeRequired(title, 150, nameof(Title));
            Body = NormalizeRequired(body, 1000, nameof(Body));
            Type = type;
            RelatedEntityID = relatedEntityId;

            IsRead = false;
            CreatedAt = DateTimeOffset.UtcNow;
        }

        // -------------------------
        // Business Actions
        // -------------------------

        public void MarkAsRead()
        {
            if (!IsRead)
            {
                IsRead = true;
            }
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
