using DeliveryApp.Application.Features.MerchantCatalog.Common;
using DeliveryApp.Application.Features.MerchantCatalog.MerchantCategories;
using DeliveryApp.Application.Features.MerchantCatalog.MerchantSystemCategories;
using DeliveryApp.Application.Features.MerchantCatalog.Products;
using DeliveryApp.Application.Features.MerchantCatalog.PublicCatalog;
using DeliveryApp.Application.Features.MerchantCatalog.SystemCategories;
using DeliveryApp.Application.Features.MerchantCatalog.Variants;
using DeliveryApp.Domain.DomainExceptions;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryApp.API.Controllers
{
    [ApiController]
    [Route("api/catalog")]
    public sealed class MerchantCatalogController : ControllerBase // Controller خاص بإدارة كتالوج المطاعم وعرضه للزبون
    {
        private readonly SystemCategoryService _systemCategoryService; // تصنيفات النظام
        private readonly MerchantSystemCategoryService _merchantSystemCategoryService; // ربط المطعم مع تصنيفات النظام
        private readonly MerchantCategoryService _merchantCategoryService; // تصنيفات المطعم
        private readonly ProductService _productService; // المنتجات
        private readonly VariantService _variantService; // خيارات المنتجات
        private readonly PublicCatalogService _publicCatalogService; // عرض الكتالوج للزبون

        public MerchantCatalogController(
            SystemCategoryService systemCategoryService,
            MerchantSystemCategoryService merchantSystemCategoryService,
            MerchantCategoryService merchantCategoryService,
            ProductService productService,
            VariantService variantService,
            PublicCatalogService publicCatalogService)
        {
            _systemCategoryService = systemCategoryService;
            _merchantSystemCategoryService = merchantSystemCategoryService;
            _merchantCategoryService = merchantCategoryService;
            _productService = productService;
            _variantService = variantService;
            _publicCatalogService = publicCatalogService;
        }

        [HttpGet("system-categories")]
        public async Task<ActionResult<IReadOnlyList<SystemCategoryDto>>> GetSystemCategories(CancellationToken ct) // جلب تصنيفات النظام
            => Ok(await _systemCategoryService.GetAllAsync(ct));

        [HttpPost("system-categories")]
        public Task<IActionResult> CreateSystemCategory([FromBody] CreateSystemCategoryRequest request, CancellationToken ct) // إنشاء تصنيف نظام
            => RunCreate(() => _systemCategoryService.CreateAsync(request, ct));

        [HttpPut("system-categories/{id:guid}")]
        public Task<IActionResult> UpdateSystemCategory(Guid id, [FromBody] UpdateSystemCategoryRequest request, CancellationToken ct) // تعديل تصنيف نظام
            => RunUpdate(() => _systemCategoryService.UpdateAsync(id, request, ct));

        [HttpPatch("system-categories/{id:guid}/activate")]
        public Task<IActionResult> ActivateSystemCategory(Guid id, CancellationToken ct) // تفعيل تصنيف نظام
            => RunChange(() => _systemCategoryService.ActivateAsync(id, ct));

        [HttpPatch("system-categories/{id:guid}/deactivate")]
        public Task<IActionResult> DeactivateSystemCategory(Guid id, CancellationToken ct) // تعطيل تصنيف نظام
            => RunChange(() => _systemCategoryService.DeactivateAsync(id, ct));

        [HttpDelete("system-categories/{id:guid}")]
        public Task<IActionResult> DeleteSystemCategory(Guid id, CancellationToken ct) // حذف تصنيف نظام
            => RunChange(() => _systemCategoryService.DeleteAsync(id, ct));

        [HttpPost("merchants/{merchantId:guid}/system-categories")]
        public Task<IActionResult> AssignMerchantSystemCategory(Guid merchantId, [FromBody] AssignMerchantSystemCategoryRequest request, CancellationToken ct) // ربط مطعم مع تصنيف نظام
            => RunChange(() => _merchantSystemCategoryService.AssignAsync(merchantId, request, ct));

        [HttpDelete("merchants/{merchantId:guid}/system-categories/{systemCategoryId:guid}")]
        public Task<IActionResult> RemoveMerchantSystemCategory(Guid merchantId, Guid systemCategoryId, CancellationToken ct) // فك ربط مطعم من تصنيف نظام
            => RunChange(() => _merchantSystemCategoryService.RemoveAsync(merchantId, systemCategoryId, ct));

        [HttpGet("merchants/{merchantId:guid}/categories")]
        public async Task<ActionResult<IReadOnlyList<MerchantCategoryDto>>> GetMerchantCategories(Guid merchantId, CancellationToken ct) // جلب تصنيفات مطعم
            => Ok(await _merchantCategoryService.GetByMerchantAsync(merchantId, ct));

        [HttpPost("merchants/{merchantId:guid}/categories")]
        public Task<IActionResult> CreateMerchantCategory(Guid merchantId, [FromBody] CreateMerchantCategoryRequest request, CancellationToken ct) // إنشاء تصنيف مطعم
            => RunCreate(() => _merchantCategoryService.CreateAsync(merchantId, request, ct));

        [HttpPut("merchant-categories/{id:guid}")]
        public Task<IActionResult> UpdateMerchantCategory(Guid id, [FromBody] UpdateMerchantCategoryRequest request, CancellationToken ct) // تعديل تصنيف مطعم
            => RunUpdate(() => _merchantCategoryService.UpdateAsync(id, request, ct));

        [HttpPatch("merchant-categories/{id:guid}/activate")]
        public Task<IActionResult> ActivateMerchantCategory(Guid id, CancellationToken ct) // تفعيل تصنيف مطعم
            => RunChange(() => _merchantCategoryService.ActivateAsync(id, ct));

        [HttpPatch("merchant-categories/{id:guid}/deactivate")]
        public Task<IActionResult> DeactivateMerchantCategory(Guid id, CancellationToken ct) // تعطيل تصنيف مطعم
            => RunChange(() => _merchantCategoryService.DeactivateAsync(id, ct));

        [HttpDelete("merchant-categories/{id:guid}")]
        public Task<IActionResult> DeleteMerchantCategory(Guid id, CancellationToken ct) // حذف تصنيف مطعم
            => RunChange(() => _merchantCategoryService.DeleteAsync(id, ct));

        [HttpGet("merchant-categories/{merchantCategoryId:guid}/products")]
        public async Task<ActionResult<IReadOnlyList<ProductDto>>> GetProducts(Guid merchantCategoryId, CancellationToken ct) // جلب منتجات تصنيف
            => Ok(await _productService.GetByCategoryAsync(merchantCategoryId, ct));

        [HttpPost("merchant-categories/{merchantCategoryId:guid}/products")]
        public Task<IActionResult> CreateProduct(Guid merchantCategoryId, [FromBody] CreateProductRequest request, CancellationToken ct) // إنشاء منتج
            => RunCreate(() => _productService.CreateAsync(merchantCategoryId, request, ct));

        [HttpPut("products/{id:guid}")]
        public Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductRequest request, CancellationToken ct) // تعديل اسم أو سعر أو صورة منتج
            => RunUpdate(() => _productService.UpdateAsync(id, request, ct));

        [HttpPatch("products/{id:guid}/activate")]
        public Task<IActionResult> ActivateProduct(Guid id, CancellationToken ct) // تفعيل منتج
            => RunChange(() => _productService.ActivateAsync(id, ct));

        [HttpPatch("products/{id:guid}/deactivate")]
        public Task<IActionResult> DeactivateProduct(Guid id, CancellationToken ct) // تعطيل منتج
            => RunChange(() => _productService.DeactivateAsync(id, ct));

        [HttpDelete("products/{id:guid}")]
        public Task<IActionResult> DeleteProduct(Guid id, CancellationToken ct) // حذف منتج
            => RunChange(() => _productService.DeleteAsync(id, ct));

        [HttpGet("products/{productId:guid}/variants")]
        public async Task<ActionResult<IReadOnlyList<VariantDto>>> GetVariants(Guid productId, CancellationToken ct) // جلب خيارات منتج
            => Ok(await _variantService.GetByProductAsync(productId, ct));

        [HttpPost("products/{productId:guid}/variants")]
        public Task<IActionResult> CreateVariant(Guid productId, [FromBody] CreateVariantRequest request, CancellationToken ct) // إنشاء خيار منتج
            => RunCreate(() => _variantService.CreateAsync(productId, request, ct));

        [HttpPut("variants/{id:guid}")]
        public Task<IActionResult> UpdateVariant(Guid id, [FromBody] UpdateVariantRequest request, CancellationToken ct) // تعديل اسم أو سعر خيار
            => RunUpdate(() => _variantService.UpdateAsync(id, request, ct));

        [HttpPatch("variants/{id:guid}/activate")]
        public Task<IActionResult> ActivateVariant(Guid id, CancellationToken ct) // تفعيل خيار
            => RunChange(() => _variantService.ActivateAsync(id, ct));

        [HttpPatch("variants/{id:guid}/deactivate")]
        public Task<IActionResult> DeactivateVariant(Guid id, CancellationToken ct) // تعطيل خيار
            => RunChange(() => _variantService.DeactivateAsync(id, ct));

        [HttpDelete("variants/{id:guid}")]
        public Task<IActionResult> DeleteVariant(Guid id, CancellationToken ct) // حذف خيار
            => RunChange(() => _variantService.DeleteAsync(id, ct));

        [HttpGet("merchants/{merchantId:guid}/public")]
        public async Task<ActionResult<MerchantCatalogDto>> GetPublicMerchantCatalog(Guid merchantId, CancellationToken ct) // إظهار منتجات المطعم للزبون
            => Ok(await _publicCatalogService.GetMerchantCatalogAsync(merchantId, ct));

        private static async Task<IActionResult> RunCreate<T>(Func<Task<T>> action) // توحيد ردود الإنشاء
        {
            try
            {
                var response = await action();
                return new OkObjectResult(response);
            }
            catch (DomainException ex)
            {
                return new BadRequestObjectResult(new { ex.Code, ex.Message });
            }
        }

        private static async Task<IActionResult> RunUpdate<T>(Func<Task<T?>> action) where T : class // توحيد ردود التعديل
        {
            try
            {
                var response = await action();
                return response is null ? new NotFoundResult() : new OkObjectResult(response);
            }
            catch (DomainException ex)
            {
                return new BadRequestObjectResult(new { ex.Code, ex.Message });
            }
        }

        private static async Task<IActionResult> RunChange(Func<Task<bool>> action) // توحيد ردود التفعيل والتعطيل والحذف
        {
            try
            {
                var changed = await action();
                return changed ? new NoContentResult() : new NotFoundResult();
            }
            catch (DomainException ex)
            {
                return new BadRequestObjectResult(new { ex.Code, ex.Message });
            }
        }
    }
}
