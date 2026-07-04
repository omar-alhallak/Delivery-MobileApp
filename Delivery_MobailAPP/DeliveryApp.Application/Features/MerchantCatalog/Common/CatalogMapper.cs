using DeliveryApp.Domain.Entities.Merchants.Catalog;

namespace DeliveryApp.Application.Features.MerchantCatalog.Common
{
    public static class CatalogMapper // يحول كائنات الدومين إلى DTOs للـ API
    {
        public static SystemCategoryDto ToDto(SystemCategory category)
        {
            return new SystemCategoryDto
            {
                Id = category.ID.Value,
                MerchantType = category.MerchantType,
                CategoryName = category.CategoryName.Value,
                Slug = category.Slug.Value,
                ImageUrl = category.ImageUrl,
                SortOrder = category.SortOrder,
                IsActive = category.IsActive,
                CreatedAt = category.CreatedAt
            };
        }

        public static MerchantCategoryDto ToDto(MerchantCategory category)
        {
            return new MerchantCategoryDto
            {
                Id = category.ID.Value,
                MerchantId = category.MerchantID.Value,
                CategoryName = category.CategoryName.Value,
                Description = category.Description,
                ImageUrl = category.ImageUrl,
                SortOrder = category.SortOrder,
                IsActive = category.IsActive,
                CreatedAt = category.CreatedAt
            };
        }

        public static ProductDto ToDto(Product product, IEnumerable<Variant>? variants = null)
        {
            return new ProductDto
            {
                Id = product.ID.Value,
                MerchantCategoryId = product.MerchantCategoryID.Value,
                ProductName = product.ProductName.Value,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                BasePrice = product.BasePrice,
                SortOrder = product.SortOrder,
                IsActive = product.IsActive,
                CreatedAt = product.CreatedAt,
                Variants = variants?.Select(ToDto).ToList() ?? []
            };
        }

        public static VariantDto ToDto(Variant variant)
        {
            return new VariantDto
            {
                Id = variant.ID.Value,
                ProductId = variant.ProductID.Value,
                VariantName = variant.VariantName.Value,
                BasePrice = variant.BasePrice,
                SortOrder = variant.SortOrder,
                IsActive = variant.IsActive,
                CreatedAt = variant.CreatedAt
            };
        }
    }
}