namespace DeliveryApp.Application.Features.MerchantCatalog.MerchantSystemCategories
{
    public sealed class AssignMerchantSystemCategoryRequest // DTO ربط مطعم مع عدة تصنيفات نظام
    {
        public List<Guid> SystemCategoryIds { get; init; } = []; // تصنيفات النظام المطلوب ربطها بالمطعم
    }
}