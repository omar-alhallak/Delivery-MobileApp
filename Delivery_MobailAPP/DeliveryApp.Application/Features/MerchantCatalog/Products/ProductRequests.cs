namespace DeliveryApp.Application.Features.MerchantCatalog.Products
{
    public sealed class CreateProductRequest // DTO إنشاء منتج
    {
        public string ProductName { get; init; } = null!; // اسم المنتج
        public string? Description { get; init; } // الوصف
        public string? ImageUrl { get; init; } // الصورة
        public decimal? BasePrice { get; init; } // السعر الأساسي إذا ما فيه variants
        public int SortOrder { get; init; } // ترتيب العرض
    }

    public sealed class UpdateProductRequest // DTO تعديل منتج
    {
        public string ProductName { get; init; } = null!; // الاسم
        public string? Description { get; init; } // الوصف
        public string? ImageUrl { get; init; } // الصورة
        public decimal? BasePrice { get; init; } // السعر الأساسي
        public int SortOrder { get; init; } // ترتيب العرض
    }
}