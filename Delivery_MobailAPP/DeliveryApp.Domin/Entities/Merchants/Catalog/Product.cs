using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.ValueObjects;
using DeliveryApp.Domain.DomainExceptions;

namespace DeliveryApp.Domain.Entities.Merchants.Catalog
{
    public class Product // يمثل المنتجات
    {
        // -------------------------
        //            Key
        // -------------------------

        public ProductID ID { get; private set; } // PK معرف المنتج

        // -------------------------
        //         Relations
        // -------------------------

        public MerchantCategoryID MerchantCategoryID { get; private set; } // التصنيف الي له المنتج

        // -------------------------
        //        Basic Info
        // -------------------------

        public DisplayName ProductName { get; private set; } = null!; // اسم المنتج
        public string? Description { get; private set; } // وصف المنتج
        public string? ImageUrl { get; private set; } // صورة المنتج

        // ملاحظة: السعر في حال عدم وجود للمنتج Variant
        public decimal? BasePrice { get; private set; } // السعر الأساسي للمنتج
        public int SortOrder { get; private set; } //ترتيب المنتجات في العرض

        // -------------------------
        //          State
        // -------------------------

        public bool IsActive { get; private set; } // هل المنتج مفعل

        // -------------------------
        //           Dates
        // -------------------------

        public DateTimeOffset CreatedAt { get; private set; } // وقت إنشاء المنتج

        private Product() { }

        public Product(ProductID id, MerchantCategoryID merchantCategoryId, string productName, string? description, int sortOrder, string? imageUrl, DateTimeOffset createdAtUtc, decimal? basePrice = null )
        {
            if (id.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(id));

            if (merchantCategoryId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(merchantCategoryId));

            if (createdAtUtc == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(createdAtUtc));
            


            ID = id;
            MerchantCategoryID = merchantCategoryId;

            SetName(productName);
            SetDescription(description);
            SetImageUrl(imageUrl);
            SetBasePrice(basePrice);
            ChangeSortOrder(sortOrder);
            IsActive = true;
            CreatedAt = createdAtUtc;
        }

        // -------------------------
        //         Behavior
        // -------------------------

        public void Rename(string name) => SetName(name); // تغيير اسم المنتج

        public void ChangeDescription(string? description) => SetDescription(description); // تغيير وصف المنتج

        public void ChangeImage(string? imageUrl) => SetImageUrl(imageUrl); // تغيير صورة المنتج

        public void ChangeBasePrice(decimal? basePrice) => SetBasePrice(basePrice); // تغيير السعر الأساسي للمنتج

        public void MoveToCategory(MerchantCategoryID newMerchantCategoryId) // نقل المنتج إلى تصنيف آخر
        {
            if (newMerchantCategoryId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(newMerchantCategoryId));

            MerchantCategoryID = newMerchantCategoryId;
        }

        public void Activate() // تفعيل المنتج
        {
            if (IsActive) return;

            IsActive = true;
        }

        public void Deactivate() // تعطيل المنتج
        {
            if (!IsActive) return;

            IsActive = false;
        }

        // -------------------------
        //           Setters
        // -------------------------

        private void SetName(string value) => ProductName = DisplayName.Create(value, 150, nameof(ProductName)); // إدخال اسم المنتج
        

        private void SetDescription(string? value) // إدخال وصف المنتج
        {
            value = NormalizeOptional(value);

            if (value is not null && value.Length > 1000) throw new DomainValidationException
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, nameof(Description));

            Description = value;
        }

        private void SetImageUrl(string? value) // إدخال صورة المنتج
        {
            value = NormalizeOptional(value);

            if (value is not null && value.Length > 500) throw new DomainValidationException
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, nameof(ImageUrl));

            ImageUrl = value;
        }

        private void SetBasePrice(decimal? value) // إدخال السعر الأساسي للمنتج
        {
            if (value.HasValue && value.Value <= 0) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(BasePrice));

            BasePrice = value;
        }

        public void ChangeSortOrder(int sortOrder)
        {
            if (sortOrder <= 0)throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode,ValidationErrors.OutOfRangeMessage,nameof(sortOrder));

            SortOrder = sortOrder;
        }

        private static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim(); // تنظيف النصوص
    }
}