namespace DeliveryApp.Application.Features.MerchantCatalog.Variants
{
    public sealed class CreateVariantRequest // DTO إنشاء خيار للمنتج
    {
        public string VariantName { get; init; } = null!; // اسم الخيار
        public decimal BasePrice { get; init; } // السعر
        public int SortOrder { get; init; } // ترتيب العرض
    }

    public sealed class UpdateVariantRequest // DTO تعديل خيار المنتج
    {
        public string VariantName { get; init; } = null!; // الاسم
        public decimal BasePrice { get; init; } // السعر
        public int SortOrder { get; init; } // ترتيب العرض
    }
}