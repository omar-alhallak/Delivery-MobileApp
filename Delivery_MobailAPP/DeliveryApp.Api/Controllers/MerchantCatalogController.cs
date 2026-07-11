using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using DeliveryApp.Domain.DomainExceptions;
using DeliveryApp.Application.Features.MerchantCatalog.Common;
using DeliveryApp.Application.Features.MerchantCatalog.Variants;
using DeliveryApp.Application.Features.MerchantCatalog.Products;
using DeliveryApp.Application.Features.MerchantCatalog.Search;
using DeliveryApp.Application.Features.MerchantCatalog.PublicCatalog;
using DeliveryApp.Application.Features.MerchantCatalog.SystemCategories;
using DeliveryApp.Application.Features.MerchantCatalog.MerchantCategories;
using DeliveryApp.Application.Features.MerchantCatalog.MerchantSystemCategories;

namespace DeliveryApp.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/catalog")]
    public sealed class MerchantCatalogController : ControllerBase // Controller خاص بإدارة كتالوج المطاعم وعرضه للزبون
    {
        private readonly SystemCategoryService _systemCategoryService; // تصنيفات النظام
        private readonly MerchantSystemCategoryService _merchantSystemCategoryService; // ربط المطعم مع تصنيفات النظام
        private readonly MerchantCategoryService _merchantCategoryService; // تصنيفات المطعم
        private readonly ProductService _productService; // المنتجات
        private readonly VariantService _variantService; // خيارات المنتجات
        private readonly PublicCatalogService _publicCatalogService; // عرض الكتالوج للزبون
        private readonly MerchantCatalogSearchService _searchService; // البحث في المطاعم وتصنيفات النظام

        public MerchantCatalogController(SystemCategoryService systemCategoryService, MerchantSystemCategoryService merchantSystemCategoryService,
            MerchantCategoryService merchantCategoryService, ProductService productService, VariantService variantService,
            PublicCatalogService publicCatalogService, MerchantCatalogSearchService searchService)
        {
            _systemCategoryService = systemCategoryService;
            _merchantSystemCategoryService = merchantSystemCategoryService;
            _merchantCategoryService = merchantCategoryService;
            _productService = productService;
            _variantService = variantService;
            _publicCatalogService = publicCatalogService;
            _searchService = searchService;
        }

        [AllowAnonymous]
        [HttpGet("system-categories")]
        public async Task<ActionResult<IReadOnlyList<SystemCategoryDto>>> GetSystemCategories(CancellationToken ct) // جلب تصنيفات النظام
            => Ok(await _systemCategoryService.GetAllAsync(ct));

        [AllowAnonymous]
        [HttpGet("search")]
        public async Task<ActionResult<MerchantCatalogSearchResponse>> Search([FromQuery] string? query, CancellationToken ct) // البحث في المطاعم وتصنيفات النظام
            => Ok(await _searchService.SearchAsync(query, ct));

        [HttpPost("merchants/{merchantId:guid}/system-categories")]
        public Task<IActionResult> AssignMerchantSystemCategory(Guid merchantId, [FromBody] AssignMerchantSystemCategoryRequest request, CancellationToken ct) // ربط مطعم مع تصنيف نظام
            => RunChange(() => _merchantSystemCategoryService.AssignAsync(GetCurrentUserId(), merchantId, request, ct));

        [HttpGet("merchants/{merchantId:guid}/system-categories")]
        public Task<IActionResult> GetMerchantSystemCategories(Guid merchantId, CancellationToken ct) // جلب تصنيفات النظام المربوطة بالمطعم
            => RunRead(() => _merchantSystemCategoryService.GetByMerchantAsync(GetCurrentUserId(), merchantId, ct));

        [HttpDelete("merchants/{merchantId:guid}/system-categories/{systemCategoryId:guid}")]
        public Task<IActionResult> RemoveMerchantSystemCategory(Guid merchantId, Guid systemCategoryId, CancellationToken ct) // فك ربط مطعم من تصنيف نظام
            => RunChange(() => _merchantSystemCategoryService.RemoveAsync(GetCurrentUserId(), merchantId, systemCategoryId, ct));

        [HttpGet("merchants/{merchantId:guid}/categories")]
        public Task<IActionResult> GetMerchantCategories(Guid merchantId, CancellationToken ct) // جلب تصنيفات مطعم
            => RunRead(() => _merchantCategoryService.GetByMerchantAsync(GetCurrentUserId(), merchantId, ct));

        [HttpPost("merchants/{merchantId:guid}/categories")]
        public Task<IActionResult> CreateMerchantCategory(Guid merchantId, [FromBody] CreateMerchantCategoryRequest request, CancellationToken ct) // إنشاء تصنيف مطعم
            => RunCreate(() => _merchantCategoryService.CreateAsync(GetCurrentUserId(), merchantId, request, ct));

        [HttpPut("merchant-categories/{id:guid}")]
        public Task<IActionResult> UpdateMerchantCategory(Guid id, [FromBody] UpdateMerchantCategoryRequest request, CancellationToken ct) // تعديل تصنيف مطعم
            => RunUpdate(() => _merchantCategoryService.UpdateAsync(GetCurrentUserId(), id, request, ct));

        [HttpPatch("merchant-categories/{id:guid}/activate")]
        public Task<IActionResult> ActivateMerchantCategory(Guid id, CancellationToken ct) // تفعيل تصنيف مطعم
            => RunChange(() => _merchantCategoryService.ActivateAsync(GetCurrentUserId(), id, ct));

        [HttpPatch("merchant-categories/{id:guid}/deactivate")]
        public Task<IActionResult> DeactivateMerchantCategory(Guid id, CancellationToken ct) // تعطيل تصنيف مطعم
            => RunChange(() => _merchantCategoryService.DeactivateAsync(GetCurrentUserId(), id, ct));

        [HttpDelete("merchant-categories/{id:guid}")]
        public Task<IActionResult> DeleteMerchantCategory(Guid id, CancellationToken ct) // حذف تصنيف مطعم
            => RunChange(() => _merchantCategoryService.DeleteAsync(GetCurrentUserId(), id, ct));

        [HttpGet("merchant-categories/{merchantCategoryId:guid}/products")]
        public Task<IActionResult> GetProducts(Guid merchantCategoryId, CancellationToken ct) // جلب منتجات تصنيف
            => RunRead(() => _productService.GetByCategoryAsync(GetCurrentUserId(), merchantCategoryId, ct));

        [HttpPost("merchant-categories/{merchantCategoryId:guid}/products")]
        public Task<IActionResult> CreateProduct(Guid merchantCategoryId, [FromBody] CreateProductRequest request, CancellationToken ct) // إنشاء منتج
            => RunCreate(() => _productService.CreateAsync(GetCurrentUserId(), merchantCategoryId, request, ct));

        [HttpPut("products/{id:guid}")]
        public Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductRequest request, CancellationToken ct) // تعديل اسم أو سعر أو صورة منتج
            => RunUpdate(() => _productService.UpdateAsync(GetCurrentUserId(), id, request, ct));

        [HttpPatch("products/{id:guid}/activate")]
        public Task<IActionResult> ActivateProduct(Guid id, CancellationToken ct) // تفعيل منتج
            => RunChange(() => _productService.ActivateAsync(GetCurrentUserId(), id, ct));

        [HttpPatch("products/{id:guid}/deactivate")]
        public Task<IActionResult> DeactivateProduct(Guid id, CancellationToken ct) // تعطيل منتج
            => RunChange(() => _productService.DeactivateAsync(GetCurrentUserId(), id, ct));

        [HttpDelete("products/{id:guid}")]
        public Task<IActionResult> DeleteProduct(Guid id, CancellationToken ct) // حذف منتج
            => RunChange(() => _productService.DeleteAsync(GetCurrentUserId(), id, ct));

        [HttpGet("products/{productId:guid}/variants")]
        public Task<IActionResult> GetVariants(Guid productId, CancellationToken ct) // جلب خيارات منتج
            => RunRead(() => _variantService.GetByProductAsync(GetCurrentUserId(), productId, ct));

        [HttpPost("products/{productId:guid}/variants")]
        public Task<IActionResult> CreateVariant(Guid productId, [FromBody] CreateVariantRequest request, CancellationToken ct) // إنشاء خيار منتج
            => RunCreate(() => _variantService.CreateAsync(GetCurrentUserId(), productId, request, ct));

        [HttpPut("variants/{id:guid}")]
        public Task<IActionResult> UpdateVariant(Guid id, [FromBody] UpdateVariantRequest request, CancellationToken ct) // تعديل اسم أو سعر خيار
            => RunUpdate(() => _variantService.UpdateAsync(GetCurrentUserId(), id, request, ct));

        [HttpPatch("variants/{id:guid}/activate")]
        public Task<IActionResult> ActivateVariant(Guid id, CancellationToken ct) // تفعيل خيار
            => RunChange(() => _variantService.ActivateAsync(GetCurrentUserId(), id, ct));

        [HttpPatch("variants/{id:guid}/deactivate")]
        public Task<IActionResult> DeactivateVariant(Guid id, CancellationToken ct) // تعطيل خيار
            => RunChange(() => _variantService.DeactivateAsync(GetCurrentUserId(), id, ct));

        [HttpDelete("variants/{id:guid}")]
        public Task<IActionResult> DeleteVariant(Guid id, CancellationToken ct) // حذف خيار
            => RunChange(() => _variantService.DeleteAsync(GetCurrentUserId(), id, ct));

        [AllowAnonymous]
        [HttpGet("merchants/{merchantId:guid}/public")]
        public async Task<ActionResult<MerchantCatalogDto>> GetPublicMerchantCatalog(Guid merchantId, CancellationToken ct) // إظهار منتجات المطعم للزبون
            => Ok(await _publicCatalogService.GetMerchantCatalogAsync(merchantId, ct));

        private Guid GetCurrentUserId()
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userId, out var parsedUserId))
                throw new UnauthorizedAccessException("Invalid user id.");

            return parsedUserId;
        }

        private static async Task<IActionResult> RunRead<T>(Func<Task<T>> action)
        {
            try
            {
                return new OkObjectResult(await action());
            }
            catch (UnauthorizedAccessException)
            {
                return new ForbidResult();
            }
            catch (DomainException ex)
            {
                return new BadRequestObjectResult(new { ex.Code, ex.Message });
            }
        }

        private static async Task<IActionResult> RunCreate<T>(Func<Task<T>> action) // توحيد ردود الإنشاء
        {
            try
            {
                var response = await action();
                return new OkObjectResult(response);
            }
            catch (UnauthorizedAccessException)
            {
                return new ForbidResult();
            }
            catch (KeyNotFoundException)
            {
                return new NotFoundResult();
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
            catch (UnauthorizedAccessException)
            {
                return new ForbidResult();
            }
            catch (KeyNotFoundException)
            {
                return new NotFoundResult();
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
            catch (UnauthorizedAccessException)
            {
                return new ForbidResult();
            }
            catch (KeyNotFoundException)
            {
                return new NotFoundResult();
            }
            catch (DomainException ex)
            {
                return new BadRequestObjectResult(new { ex.Code, ex.Message });
            }
        }
    }
}
