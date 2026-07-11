using DeliveryApp.Application.Features.MerchantCatalog.Common;

namespace DeliveryApp.Application.Features.MerchantCatalog.Search
{
    public sealed class MerchantCatalogSearchResponse // نتيجة البحث العامة في الكتالوج
    {
        public string Query { get; init; } = null!; // النص الذي تم البحث عنه
        public IReadOnlyList<MerchantCatalogSearchMerchantDto> Merchants { get; init; } = []; // المطاعم المطابقة
        public IReadOnlyList<SystemCategoryDto> SystemCategories { get; init; } = []; // تصنيفات النظام المطابقة
    }

    public sealed class MerchantCatalogSearchMerchantDto // نتيجة مطعم ضمن البحث
    {
        public Guid MerchantId { get; init; } // معرف المطعم
        public string PublicId { get; init; } = null!; // الكود العام للمطعم
        public string MerchantName { get; init; } = null!; // اسم المطعم
        public string Slug { get; init; } = null!; // الاسم المختصر
        public string? LogoUrl { get; init; } // شعار المطعم
        public string? CoverImageUrl { get; init; } // صورة الغلاف
        public decimal AverageRating { get; init; } // متوسط التقييم
        public int RatingsCount { get; init; } // عدد التقييمات
        public int DefaultPreparationMinutes { get; init; } // وقت التحضير الافتراضي بالدقائق
    }
}
