using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.DomainExceptions;
using DeliveryApp.Domain.Enums.MerchantEnums;

namespace DeliveryApp.Domain.Entities.Merchants
{
    public class MerchantUser // جدول ربط بين المستخدمين والمطاعم وتحديد صلاحيات كل مستخدم داخل المطعم
    {
        // -------------------------
        //         Relations
        // -------------------------

        public MerchantID MerchantID { get; private set; } // المطعم المرتبط
        public UserID UserID { get; private set; } // المستخدم المرتبط

        // -------------------------
        //          Access
        // -------------------------

        public MerchantUserRole Role { get; private set; } // صلاحية المستخدم داخل المطعم

        // -------------------------
        //          State
        // -------------------------

        public bool IsActive { get; private set; } // هل العلاقة فعالة

        // -------------------------
        //           Dates
        // -------------------------

        public DateTimeOffset CreatedAt { get; private set; } // وقت إنشاء الربط

        private MerchantUser() { }

        public MerchantUser(MerchantID merchantId, UserID userId, MerchantUserRole role, DateTimeOffset createdAtUtc)
        {
            if (merchantId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(merchantId));

            if (userId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(userId));

            if (createdAtUtc == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(createdAtUtc));

            if (!Enum.IsDefined(typeof(MerchantUserRole), role)) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(role));

            MerchantID = merchantId;
            UserID = userId;
            Role = role;
            CreatedAt = createdAtUtc;
            IsActive = true;
        }

        // -------------------------
        //         Behavior
        // -------------------------

        public void ChangeRole(MerchantUserRole role) // تغيير دور المستخدم داخل المطعم
        {
            if (!Enum.IsDefined(typeof(MerchantUserRole), role)) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(role));

            if (Role == role) return;

            Role = role;
        }

        public void Activate() // تفعيل العلاقة بين المستخدم والمطعم
        {
            if (IsActive) return;

            IsActive = true;
        }

        public void Deactivate() // تعطيل العلاقة بين المستخدم والمطعم
        {
            if (!IsActive) return;

            IsActive = false;
        }
    }
}