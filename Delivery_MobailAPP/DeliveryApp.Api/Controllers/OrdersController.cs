using Microsoft.AspNetCore.Mvc;
using DeliveryApp.Domain.DomainExceptions;
using DeliveryApp.Application.Features.Orders.Common;
using DeliveryApp.Application.Features.Orders.Payment;
using DeliveryApp.Application.Features.Orders.GetOrders;
using DeliveryApp.Application.Features.Orders.CancelOrder;
using DeliveryApp.Application.Features.Orders.CreateOrder;
using DeliveryApp.Application.Features.Orders.OrderWorkflow;
using DeliveryApp.Application.Features.Orders.MerchantDecision;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace DeliveryApp.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/orders")]
    public sealed class OrdersController : ControllerBase // Controller يفتح endpoints الخاصة بالطلبات للفرونت
    {
        private readonly OrderQueryService _orderQueryService; 
        private readonly CreateOrderService _createOrderService; 
        private readonly OrderWorkflowService _orderWorkflowService; 
        private readonly MerchantDecisionService _merchantDecisionService; 
        private readonly CancelOrderService _cancelOrderService; 
        private readonly OrderPaymentService _orderPaymentService;

        public OrdersController(OrderQueryService orderQueryService, CreateOrderService createOrderService,
            OrderWorkflowService orderWorkflowService, MerchantDecisionService merchantDecisionService, CancelOrderService cancelOrderService, OrderPaymentService orderPaymentService)
        {
            _orderQueryService = orderQueryService;
            _createOrderService = createOrderService;
            _orderWorkflowService = orderWorkflowService;
            _merchantDecisionService = merchantDecisionService;
            _cancelOrderService = cancelOrderService;
            _orderPaymentService = orderPaymentService;
        }

        [HttpGet("my")]
        public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetMyOrders(CancellationToken ct) // طلبات الزبون الحالي فقط
        {
            var response = await _orderQueryService.GetForCustomerAsync(GetCurrentUserId(), ct);
            return Ok(response);
        }

        [HttpGet("merchant/{merchantId:guid}")]
        public async Task<IActionResult> GetMerchantOrders(Guid merchantId, CancellationToken ct) // طلبات مطعم يديره المستخدم الحالي
        {
            try
            {
                var response = await _orderQueryService.GetForMerchantAsync(GetCurrentUserId(), merchantId, ct);
                return Ok(response);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<OrderDto>> GetById(Guid id, CancellationToken ct) // GET /api/orders/{id}
        {
            try
            {
                var response = await _orderQueryService.GetByIdAsync(GetCurrentUserId(), id, ct);
                return response is null ? NotFound() : Ok(response);
            }
            catch (UnauthorizedAccessException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<ActionResult<OrderDto>> Create([FromBody] CreateOrderRequest request, CancellationToken ct) // POST /api/orders
        {
            try
            {
                var response = await _createOrderService.ExecuteAsync(GetCurrentUserId(), request, ct);
                return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
            }
            catch (DomainValidationException ex)
            {
                return BadRequest(new { ex.Code, ex.Message, ex.Field });
            }
            catch (DomainException ex)
            {
                return BadRequest(new { ex.Code, ex.Message });
            }
        }

        // الحذف النهائي متوقف حتى ما ينحذف السجل المالي للطلب. الإلغاء يتم من endpoint cancel.
        // [HttpDelete("{id:guid}")]
        // public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        // {
        //     var deleted = await _deleteOrderService.ExecuteAsync(id, ct);
        //     return deleted ? NoContent() : NotFound();
        // }

        //[HttpPatch("{id:guid}/submit-to-merchant")]
        //public Task<IActionResult> SubmitToMerchant(Guid id, CancellationToken ct) // Endpoint مساعد للطلبات القديمة فقط، ويمكن حذفه لاحقاً إذا صار POST يرسل الطلب دائماً للمطعم
        //    => RunOrderChange(() => _orderWorkflowService.SubmitToMerchantAsync(id, ct));

        [HttpPatch("{id:guid}/approve-by-merchant")]
        public Task<IActionResult> ApproveByMerchant(Guid id, CancellationToken ct) // موافقة المطعم
            => RunOrderChange(() => _merchantDecisionService.ApproveAsync(GetCurrentUserId(), id, ct));

        [HttpPatch("{id:guid}/reject-by-merchant")]
        public Task<IActionResult> RejectByMerchant(Guid id, [FromBody] MerchantDecisionRequest request, CancellationToken ct) // رفض المطعم مع السبب
            => RunOrderChange(() => _merchantDecisionService.RejectAsync(GetCurrentUserId(), id, request, ct));

        [HttpPatch("{id:guid}/ready-for-pickup")]
        public Task<IActionResult> ReadyForPickup(Guid id, CancellationToken ct) // المطعم جهز الطلب
            => RunOrderChange(() => _orderWorkflowService.ReadyForPickupAsync(GetCurrentUserId(), id, ct));

        [HttpPatch("{id:guid}/picked-up")]
        public Task<IActionResult> PickedUp(Guid id, CancellationToken ct) // السائق استلم الطلب
            => RunOrderChange(() => _orderWorkflowService.PickedUpAsync(GetCurrentUserId(), id, ct));

        // لم نعد نحتاج هذا endpoint لأن picked-up ينقل الطلب مباشرة إلى OnTheWay.
        // [HttpPatch("{id:guid}/on-the-way")]
        // public Task<IActionResult> OnTheWay(Guid id, CancellationToken ct)
        //     => RunOrderChange(() => _orderWorkflowService.OnTheWayAsync(GetCurrentUserId(), id, ct));

        [HttpPatch("{id:guid}/delivered")]
        public Task<IActionResult> Delivered(Guid id, CancellationToken ct) // الطلب وصل للزبون
            => RunOrderChange(() => _orderWorkflowService.DeliveredAsync(GetCurrentUserId(), id, ct));

        [HttpPatch("{id:guid}/cancel")]
        public Task<IActionResult> Cancel(Guid id, [FromBody] CancelOrderRequest request, CancellationToken ct) // إلغاء الطلب
            => RunOrderChange(() => _cancelOrderService.ExecuteAsync(GetCurrentUserId(), id, request, ct));

        [HttpPatch("{id:guid}/paid")]
        public Task<IActionResult> MarkAsPaid(Guid id, CancellationToken ct) // تعليم الطلب مدفوع
            => RunOrderChange(() => _orderPaymentService.MarkAsPaidAsync(GetCurrentUserId(), id, ct));

        // الدفع نقدي فقط، وما منرجع الطلب إلى غير مدفوع من الواجهة.
        // [HttpPatch("{id:guid}/unpaid")]
        // public Task<IActionResult> MarkAsUnpaid(Guid id, CancellationToken ct)
        //     => RunOrderChange(() => _orderPaymentService.MarkAsUnpaidAsync(GetCurrentUserId(), id, ct));

        private Guid GetCurrentUserId()
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userId, out var parsedUserId))
                throw new UnauthorizedAccessException("Invalid user id.");

            return parsedUserId;
        }

        private static async Task<IActionResult> RunOrderChange(Func<Task<bool>> change) // دالة مشتركة لتوحيد ردود تعديل حالة الطلب
        { 
            try
            {
                var changed = await change(); // true يعني الطلب موجود وتعدل
                return changed ? new NoContentResult() : new NotFoundResult();
            }
            catch (DomainValidationException ex)
            {
                return new BadRequestObjectResult(new { ex.Code, ex.Message, ex.Field });
            }
            catch (DomainException ex)
            {
                return new BadRequestObjectResult(new { ex.Code, ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return new ForbidResult();
            }
        }
    }
}
