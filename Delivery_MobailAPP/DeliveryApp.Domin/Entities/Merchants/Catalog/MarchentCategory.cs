using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.ValueObjects;
using DeliveryApp.Domain.DomainExceptions;

namespace DeliveryApp.Domain.Entities.Merchants.Catalog
{
    public class MerchantCategory // يمثل تصنيفات الخاصة بالتجار
    {
        // -------------------------
        //            Key
        // -------------------------

        public MerchantCategoryID ID { get; private set; } // PK معرف التصنيف
        public MerchantID MerchantID { get; private set; } // المطعم الي بيملك هذا التصنيف

        // -------------------------
        //       Basic Info
        // -------------------------

        public DisplayName CategoryName { get; private set; } = null!; // اسم التصنيف
        public string? Description { get; private set; } // وصف اختياري للتصنيف
        public string? ImageUrl { get; private set; } // صورة التصنيف

        // -------------------------
        //        Display
        // -------------------------

        public int SortOrder { get; private set; } // ترتيب عرض التصنيف داخل المطعم
        public bool IsActive { get; private set; } // هل التصنيف مفعل ويظهر للمستخدم

        // -------------------------
        //          Dates
        // -------------------------

        public DateTimeOffset CreatedAt { get; private set; } // وقت إنشاء التصنيف

        private MerchantCategory() { }

        public MerchantCategory(MerchantCategoryID id, MerchantID merchantId, string categoryName, string? description, string? imageUrl, int sortOrder, DateTimeOffset createdAtUtc)
        {
            if (id.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(id));

            if (merchantId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(merchantId));

            if (createdAtUtc == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(createdAtUtc));

            ID = id;
            MerchantID = merchantId;

            SetName(categoryName);
            SetDescription(description);
            SetImageUrl(imageUrl);
            SetSortOrder(sortOrder);

            IsActive = true;
            CreatedAt = createdAtUtc;
        }

        // -------------------------
        //          Behavior
        // -------------------------

        public void Rename(string name) => SetName(name); // تغيير اسم التصنيف

        public void ChangeDescription(string? description) => SetDescription(description); // تغيير وصف التصنيف

        public void ChangeImage(string? imageUrl) => SetImageUrl(imageUrl); // تغيير صورة التصنيف

        public void ChangeSortOrder(int sortOrder) => SetSortOrder(sortOrder); // تغيير ترتيب التصنيف

        public void Activate() // تفعيل التصنيف
        {
            if (IsActive) return;

            IsActive = true;
        }

        public void Deactivate() // تعطيل التصنيف
        {
            if (!IsActive) return;

            IsActive = false;
        }

        // -------------------------
        //          Setters
        // -------------------------

        private void SetName(string value) => CategoryName = DisplayName.Create(value, 150, nameof(CategoryName)); // إدخال اسم التصنيف

        private void SetDescription(string? value) // إدخال وصف التصنيف
        {
            value = NormalizeOptional(value);

            if (value is not null && value.Length > 500) throw new DomainValidationException
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, nameof(Description));

            Description = value;
        }

        private void SetImageUrl(string? value) // إدخال صورة التصنيف
        {
            value = NormalizeOptional(value);

            if (value is not null && value.Length > 500) throw new DomainValidationException
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, nameof(ImageUrl));

            ImageUrl = value;
        }

        private void SetSortOrder(int value) // إدخال ترتيب التصنيف
        {
            if (value < 0) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(SortOrder));

            SortOrder = value;
        }

        private static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim(); // تنظيف النصوص
    }
}