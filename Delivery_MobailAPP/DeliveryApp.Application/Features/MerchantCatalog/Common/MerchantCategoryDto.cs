namespace DeliveryApp.Application.Features.MerchantCatalog.Common
{
    public sealed class MerchantCategoryDto // DTO يرجع تصنيف المطعم للواجهة
    {
        public Guid Id { get; init; } // معرف التصنيف
        public Guid MerchantId { get; init; } // معرف المطعم
        public string CategoryName { get; init; } = null!; // اسم التصنيف
        public string? Description { get; init; } // وصف التصنيف
        public string? ImageUrl { get; init; } // صورة التصنيف
        public int SortOrder { get; init; } // ترتيب العرض
        public bool IsActive { get; init; } // هل التصنيف مفعل
        public DateTimeOffset CreatedAt { get; init; } // وقت الإنشاء
    }
}
