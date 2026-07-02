using DeliveryApp.Application.Features.Addresses.AddressLifecycle;
using DeliveryApp.Application.Features.Addresses.AddressQuery;
using DeliveryApp.Application.Features.Addresses.AddressStatus;
using DeliveryApp.Application.Features.Addresses.Common;
using DeliveryApp.Application.Features.Addresses.CompleteAddress;
using DeliveryApp.Application.Features.Addresses.CreateAddressLocation;
using DeliveryApp.Application.Features.Addresses.UpdateAddressDetails;
using DeliveryApp.Application.Features.Addresses.UpdateAddressLocation;
using DeliveryApp.Domain.DomainExceptions;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryApp.API.Controllers
{
    [ApiController]
    [Route("api/addresses")]
    public sealed class AddressesController : ControllerBase // Controller خاص بعناوين المستخدم
    {
        private readonly AddressQueryService _queryService; // قراءة العناوين
        private readonly AddressLifecycleService _lifecycleService; // إنشاء وإكمال وتعديل العنوان
        private readonly AddressStatusService _statusService; // default و activate/deactivate

        public AddressesController(
            AddressQueryService queryService,
            AddressLifecycleService lifecycleService,
            AddressStatusService statusService)
        {
            _queryService = queryService;
            _lifecycleService = lifecycleService;
            _statusService = statusService;
        }

        [HttpGet("users/{userId:guid}")]
        public async Task<ActionResult<IReadOnlyList<AddressDto>>> GetByUser(Guid userId, CancellationToken ct) // جلب عناوين مستخدم
        {
            var response = await _queryService.GetByUserAsync(userId, ct);
            return Ok(response);
        }

        [HttpGet("{id:guid}")]
        public Task<IActionResult> GetById(Guid id, CancellationToken ct) // جلب عنوان واحد
            => RunNullable(() => _queryService.GetByIdAsync(id, ct));

        [HttpPost("location")]
        public Task<IActionResult> CreateLocation([FromBody] CreateAddressLocationRequest request, CancellationToken ct) // إنشاء عنوان مؤقت من الخريطة
            => RunCreate(() => _lifecycleService.CreateLocationAsync(request, ct));

        [HttpPatch("{id:guid}/complete")]
        public Task<IActionResult> Complete(Guid id, [FromBody] CompleteAddressRequest request, CancellationToken ct) // إكمال تفاصيل العنوان
            => RunNullable(() => _lifecycleService.CompleteAsync(id, request, ct));

        [HttpPatch("{id:guid}/details")]
        public Task<IActionResult> UpdateDetails(Guid id, [FromBody] UpdateAddressDetailsRequest request, CancellationToken ct) // تعديل تفاصيل العنوان
            => RunNullable(() => _lifecycleService.UpdateDetailsAsync(id, request, ct));

        [HttpPatch("{id:guid}/location")]
        public Task<IActionResult> UpdateLocation(Guid id, [FromBody] UpdateAddressLocationRequest request, CancellationToken ct) // تعديل موقع العنوان المؤقت فقط
            => RunNullable(() => _lifecycleService.UpdateLocationAsync(id, request, ct));

        [HttpPatch("{id:guid}/default")]
        public Task<IActionResult> SetDefault(Guid id, CancellationToken ct) // جعل العنوان default
            => RunNullable(() => _statusService.SetDefaultAsync(id, ct));

        [HttpPatch("{id:guid}/activate")]
        public Task<IActionResult> Activate(Guid id, CancellationToken ct) // تفعيل العنوان
            => RunNullable(() => _statusService.ActivateAsync(id, ct));

        [HttpPatch("{id:guid}/deactivate")]
        public Task<IActionResult> Deactivate(Guid id, CancellationToken ct) // تعطيل العنوان بدل حذفه
            => RunNullable(() => _statusService.DeactivateAsync(id, ct));

        private static async Task<IActionResult> RunCreate<T>(Func<Task<T>> action)
        {
            try
            {
                return new OkObjectResult(await action());
            }
            catch (DomainException ex)
            {
                return new BadRequestObjectResult(new { ex.Code, ex.Message });
            }
        }

        private static async Task<IActionResult> RunNullable<T>(Func<Task<T?>> action) where T : class
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
    }
}
