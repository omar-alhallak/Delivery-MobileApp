namespace DeliveryApp.Application.Features.MerchantCatalog.Common
{
    public sealed class MerchantCatalogDto // DTO يعرض كتالوج المطعم للزبون
    {
        public Guid MerchantId { get; init; } // معرف المطعم
        public IReadOnlyList<MerchantCatalogCategoryDto> Categories { get; init; } = []; // التصنيفات الفعالة
    }

    public sealed class MerchantCatalogCategoryDto // تصنيف داخل كتالوج المطعم
    {
        public Guid Id { get; init; } // معرف التصنيف
        public string CategoryName { get; init; } = null!; // اسم التصنيف
        public string? Description { get; init; } // الوصف
        public string? ImageUrl { get; init; } // الصورة
        public int SortOrder { get; init; } // ترتيب العرض
        public IReadOnlyList<ProductDto> Products { get; init; } = []; // المنتجات الفعالة
    }
}
