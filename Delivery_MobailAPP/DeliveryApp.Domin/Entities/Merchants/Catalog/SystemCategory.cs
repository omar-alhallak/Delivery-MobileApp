using DeliveryApp.Domain.ValueObjects;
using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.DomainExceptions;
using DeliveryApp.Domain.Enums.MerchantEnums;

namespace DeliveryApp.Domain.Entities.Merchants.Catalog
{
    public class SystemCategory // يمثل تصنيفات نظام مثل (مطاعم أو متاجر الخ)د
    {
        // -------------------------
        //            Key
        // -------------------------

        public SystemCategoryID ID { get; private set; } // PK معرف التصنيف 
        public MerchantType MerchantType { get; private set; } // نوع التاجر الي قله هذا التصنيف

        // -------------------------
        //        Basic Info
        // -------------------------

        public CatalogName CategoryName { get; private set; } = null!; // اسم التصنيف
        public Slug Slug { get; private set; } = null!; // الاسم المختصر المستخدم في الروابط

        public string? ImageUrl { get; private set; } // صورة التصنيف

        // -------------------------
        //         Display
        // -------------------------

        public int SortOrder { get; private set; } // ترتيب التصنيف في العرض
        public bool IsActive { get; private set; } // هل التصنيف مفعل

        // -------------------------
        //           Dates
        // -------------------------

        public DateTimeOffset CreatedAt { get; private set; } // وقت إنشاء التصنيف

        private SystemCategory() { }

        public SystemCategory(SystemCategoryID id, MerchantType merchantType, string categoryName, string slug, string? imageUrl, int sortOrder, DateTimeOffset createdAtUtc)
        {
            if (id.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(id));

            if (!Enum.IsDefined(typeof(MerchantType), merchantType)) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(merchantType));

            if (createdAtUtc == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(createdAtUtc));

            ID = id;
            MerchantType = merchantType;

            SetName(categoryName);
            SetSlug(slug);
            SetImageUrl(imageUrl);
            SetSortOrder(sortOrder);

            IsActive = true;
            CreatedAt = createdAtUtc;
        }

        // -------------------------
        //         Behavior
        // -------------------------

        public void Rename(string categoryName) => SetName(categoryName); // تغيير اسم التصنيف

        public void ChangeSlug(string slug) => SetSlug(slug); // تغيير الـ Slug

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

        private void SetName(string value) => CategoryName = CatalogName.Create(value, 150, nameof(CategoryName)); // إدخال اسم التصنيف باستخدام

        private void SetSlug(string value) => Slug = Slug.Create(value); // إدخال ال Slug

        private void SetImageUrl(string? value) // إدخال صورة التصنيف
        {
            value = NormalizeOptional(value);

            if (value is not null && value.Length > 500) throw new DomainValidationException
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, nameof(ImageUrl));

            ImageUrl = value;
        }

        private void SetSortOrder(int value) // إدخال ترتيب العرض
        {
            if (value < 0) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(SortOrder));

            SortOrder = value;
        }

        private static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim(); // تنظيف النصوص
    }
}