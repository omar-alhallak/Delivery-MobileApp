namespace DeliveryApp.Application.Features.MerchantCatalog.Common
{
    public sealed class ProductDto // DTO يرجع المنتج للواجهة
    {
        public Guid Id { get; init; } // معرف المنتج
        public Guid MerchantCategoryId { get; init; } // التصنيف الذي يحتوي المنتج
        public string ProductName { get; init; } = null!; // اسم المنتج
        public string? Description { get; init; } // وصف المنتج
        public string? ImageUrl { get; init; } // صورة المنتج
        public decimal? BasePrice { get; init; } // سعر المنتج إذا ما عنده variants
        public int SortOrder { get; init; } // ترتيب العرض
        public bool IsActive { get; init; } // هل المنتج مفعل
        public DateTimeOffset CreatedAt { get; init; } // وقت الإنشاء
        public IReadOnlyList<VariantDto> Variants { get; init; } = []; // خيارات المنتج
    }
}