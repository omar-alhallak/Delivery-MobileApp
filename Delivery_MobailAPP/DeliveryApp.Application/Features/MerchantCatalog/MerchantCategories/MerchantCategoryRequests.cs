namespace DeliveryApp.Application.Features.MerchantCatalog.MerchantCategories
{
    public sealed class CreateMerchantCategoryRequest // DTO إنشاء تصنيف داخل مطعم
    {
        public string CategoryName { get; init; } = null!; // اسم التصنيف
        public string? Description { get; init; } // الوصف
        public string? ImageUrl { get; init; } // الصورة
        public int SortOrder { get; init; } // ترتيب العرض
    }

    public sealed class UpdateMerchantCategoryRequest // DTO تعديل تصنيف داخل مطعم
    {
        public string CategoryName { get; init; } = null!; // الاسم
        public string? Description { get; init; } // الوصف
        public string? ImageUrl { get; init; } // الصورة
        public int SortOrder { get; init; } // ترتيب العرض
    }
}
