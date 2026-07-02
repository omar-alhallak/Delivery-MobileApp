namespace DeliveryApp.Application.Features.MerchantCatalog.MerchantSystemCategories
{
    public sealed class AssignMerchantSystemCategoryRequest // DTO ربط مطعم مع تصنيف نظام
    {
        public Guid SystemCategoryId { get; init; } // تصنيف النظام المطلوب ربطه بالمطعم
    }
}
