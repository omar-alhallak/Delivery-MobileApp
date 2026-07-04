using DeliveryApp.Domain.Enums.MerchantEnums;

namespace DeliveryApp.Application.Features.MerchantCatalog.Common
{
    public sealed class SystemCategoryDto // DTO يرجع تصنيف النظام للواجهة
    {
        public Guid Id { get; init; } // معرف التصنيف
        public MerchantType MerchantType { get; init; } // نوع التاجر
        public string CategoryName { get; init; } = null!; // اسم التصنيف
        public string Slug { get; init; } = null!; // الاسم المختصر
        public string? ImageUrl { get; init; } // صورة التصنيف
        public int SortOrder { get; init; } // ترتيب العرض
        public bool IsActive { get; init; } // هل التصنيف مفعل
        public DateTimeOffset CreatedAt { get; init; } // وقت الإنشاء
    }
}