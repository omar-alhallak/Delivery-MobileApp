using DeliveryApp.Domain.Enums.MerchantEnums;

namespace DeliveryApp.Application.Features.MerchantCatalog.SystemCategories
{
    public sealed class CreateSystemCategoryRequest // DTO إنشاء تصنيف نظام
    {
        public MerchantType MerchantType { get; init; } // نوع التاجر
        public string CategoryName { get; init; } = null!; // اسم التصنيف
        public string Slug { get; init; } = null!; // الاسم المختصر
        public string? ImageUrl { get; init; } // صورة التصنيف
        public int SortOrder { get; init; } // ترتيب العرض
    }

    public sealed class UpdateSystemCategoryRequest // DTO تعديل تصنيف نظام
    {
        public string CategoryName { get; init; } = null!; // الاسم الجديد
        public string Slug { get; init; } = null!; // الاسم المختصر الجديد
        public string? ImageUrl { get; init; } // الصورة الجديدة
        public int SortOrder { get; init; } // ترتيب العرض
    }
}