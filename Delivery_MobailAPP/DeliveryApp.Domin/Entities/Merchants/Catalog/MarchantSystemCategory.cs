using System;
using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.DomainExceptions;

namespace DeliveryApp.Domain.Entities.Merchants.Catalog
{
    public class MerchantSystemCategory
    {
        public MerchantID MerchantID { get; private set; }
        public SystemCategoryID SystemCategoryID { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; }

        private MerchantSystemCategory() { }

        public MerchantSystemCategory(MerchantID MerchantId, SystemCategoryID SystemCategoryId, DateTimeOffset CreatedAtUtc)
        {
            if (MerchantId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(MerchantId));

            if (SystemCategoryId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(SystemCategoryId));

            MerchantID = MerchantId;
            SystemCategoryID = SystemCategoryId;
            CreatedAt = CreatedAtUtc;
        }
    }
}