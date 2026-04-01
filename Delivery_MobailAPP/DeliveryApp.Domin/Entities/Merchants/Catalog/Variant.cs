using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.ValueObjects;
using DeliveryApp.Domain.DomainExceptions;

namespace DeliveryApp.Domain.Entities.Merchants.Catalog
{
    public class Variant // (يمثل تفاصيل المنتجات مثل (الحجم أو سعة الخ
    {
        // -------------------------
        //            Key
        // -------------------------

        public VariantID ID { get; private set; } // PK معرف ال Variant
        public ProductID ProductID { get; private set; } // المنتج المرتبط فيه

        // -------------------------
        //        Basic Info
        // -------------------------

        public CatalogName VariantName { get; private set; } = null!; // اسم ال Variant
        public decimal BasePrice { get; private set; } // السعر الأساسي

        // -------------------------
        //          State
        // -------------------------

        public bool IsActive { get; private set; } // مفعل أو لاء
        public DateTimeOffset CreatedAt { get; private set; } // وقت الإنشاء

        private Variant() { }

        public Variant(VariantID variantId, ProductID productId, string variantName, decimal basePrice, DateTimeOffset createdAtUtc)
        {
            if (variantId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(variantId));

            if (productId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(productId));

            if (createdAtUtc == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(createdAtUtc));

            ID = variantId;
            ProductID = productId;

            SetName(variantName);
            SetBasePrice(basePrice);

            IsActive = true;
            CreatedAt = createdAtUtc;
        }

        // -------------------------
        //          Behavior
        // -------------------------

        public void Rename(string name) => SetName(name); // تغيير اسم ال Variant

        public void ChangePrice(decimal basePrice) => SetBasePrice(basePrice); // تغيير السعر

        public void Activate() // تفعيل ال Variant
        {
            if (IsActive) return;

            IsActive = true;
        }

        public void Deactivate() // تعطيل ال Variant
        {
            if (!IsActive) return;

            IsActive = false;
        }

        // -------------------------
        //           Setters
        // -------------------------

        private void SetName(string value) => VariantName = CatalogName.Create(value, 100, nameof(VariantName)); // إدخال اسم ال Variant
        
        private void SetBasePrice(decimal value) // إدخال السعر
        {
            if (value < 0) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(BasePrice));

            BasePrice = value;
        }
    }
}