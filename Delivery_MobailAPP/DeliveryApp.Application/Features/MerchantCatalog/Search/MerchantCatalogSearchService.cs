using DeliveryApp.Application.Features.MerchantCatalog.Common;
using DeliveryApp.Application.Interfaces.MerchantCatalogRepositoriesInterfaces;

namespace DeliveryApp.Application.Features.MerchantCatalog.Search
{
    public sealed class MerchantCatalogSearchService // Use case البحث في المطاعم وتصنيفات النظام
    {
        private const int MaxResultsPerSection = 10;

        private readonly IMerchantCatalogReadRepository _repository; // Repository قراءة الكتالوج

        public MerchantCatalogSearchService(IMerchantCatalogReadRepository repository)
        {
            _repository = repository;
        }

        public async Task<MerchantCatalogSearchResponse> SearchAsync(string? query, CancellationToken ct = default) // تنفيذ البحث حسب النص المدخل
        {
            var normalizedQuery = query?.Trim() ?? string.Empty;

            if (normalizedQuery.Length == 0)
            {
                return new MerchantCatalogSearchResponse
                {
                    Query = normalizedQuery
                };
            }

            var merchants = await _repository.SearchActiveMerchantsAsync(normalizedQuery, MaxResultsPerSection, ct);
            var categories = await _repository.SearchActiveSystemCategoriesAsync(normalizedQuery, MaxResultsPerSection, ct);

            return new MerchantCatalogSearchResponse
            {
                Query = normalizedQuery,
                Merchants = merchants,
                SystemCategories = categories.Select(CatalogMapper.ToDto).ToList()
            };
        }
    }
}
