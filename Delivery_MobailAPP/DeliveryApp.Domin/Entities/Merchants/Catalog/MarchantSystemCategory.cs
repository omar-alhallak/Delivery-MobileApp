using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.DomainExceptions;

namespace DeliveryApp.Domain.Entities.Merchants.Catalog
{
    public class MerchantSystemCategory //جدول ربط بين مطاعم تصنيفات نظام
    {
        // -------------------------
        //         Relations
        // -------------------------

        public MerchantID MerchantID { get; private set; } // المطعم المرتبط
        public SystemCategoryID SystemCategoryID { get; private set; } // التصنيفات النظام المرتبط

        // -------------------------
        //           Dates
        // -------------------------

        public DateTimeOffset CreatedAt { get; private set; } // وقت إنشاء هذا الربط

        private MerchantSystemCategory() { }

        public MerchantSystemCategory(MerchantID merchantId, SystemCategoryID systemCategoryId, DateTimeOffset createdAtUtc)
        {
            if (merchantId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(merchantId));

            if (systemCategoryId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(systemCategoryId));

            if (createdAtUtc == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(createdAtUtc));

            MerchantID = merchantId;
            SystemCategoryID = systemCategoryId;
            CreatedAt = createdAtUtc;
        }
    }
}