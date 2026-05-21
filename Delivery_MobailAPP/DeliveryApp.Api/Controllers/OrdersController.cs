using DeliveryApp.Application.Features.Orders.CancelOrder;
using DeliveryApp.Application.Features.Orders.Common;
using DeliveryApp.Application.Features.Orders.CreateOrder;
using DeliveryApp.Application.Features.Orders.DeleteOrder;
using DeliveryApp.Application.Features.Orders.GetOrders;
using DeliveryApp.Application.Features.Orders.MerchantDecision;
using DeliveryApp.Application.Features.Orders.OrderWorkflow;
using DeliveryApp.Application.Features.Orders.Payment;
using DeliveryApp.Domain.DomainExceptions;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryApp.API.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public sealed class OrdersController : ControllerBase // Controller يفتح endpoints الخاصة بالطلبات للفرونت
    {
        private readonly OrderQueryService _orderQueryService; // قراءة الطلبات
        private readonly CreateOrderService _createOrderService; // إنشاء طلب
        private readonly DeleteOrderService _deleteOrderService; // حذف طلب
        private readonly OrderWorkflowService _orderWorkflowService; // خطوات دورة حياة الطلب
        private readonly MerchantDecisionService _merchantDecisionService; // قبول أو رفض المطعم
        private readonly CancelOrderService _cancelOrderService; // إلغاء الطلب
        private readonly OrderPaymentService _orderPaymentService; // تعديل حالة الدفع

        public OrdersController(
            OrderQueryService orderQueryService,
            CreateOrderService createOrderService,
            DeleteOrderService deleteOrderService,
            OrderWorkflowService orderWorkflowService,
            MerchantDecisionService merchantDecisionService,
            CancelOrderService cancelOrderService,
            OrderPaymentService orderPaymentService)
        {
            _orderQueryService = orderQueryService;
            _createOrderService = createOrderService;
            _deleteOrderService = deleteOrderService;
            _orderWorkflowService = orderWorkflowService;
            _merchantDecisionService = merchantDecisionService;
            _cancelOrderService = cancelOrderService;
            _orderPaymentService = orderPaymentService;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetAll(CancellationToken ct) // GET /api/orders
        {
            var response = await _orderQueryService.GetAllAsync(ct);
            return Ok(response);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<OrderDto>> GetById(Guid id, CancellationToken ct) // GET /api/orders/{id}
        {
            var response = await _orderQueryService.GetByIdAsync(id, ct);
            return response is null ? NotFound() : Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<OrderDto>> Create([FromBody] CreateOrderRequest request, CancellationToken ct) // POST /api/orders
        {
            try
            {
                var response = await _createOrderService.ExecuteAsync(request, ct);
                return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
            }
            catch (DomainException ex)
            {
                return BadRequest(new { ex.Code, ex.Message });
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct) // DELETE /api/orders/{id}
        {
            var deleted = await _deleteOrderService.ExecuteAsync(id, ct);
            return deleted ? NoContent() : NotFound();
        }

        //[HttpPatch("{id:guid}/submit-to-merchant")]
        //public Task<IActionResult> SubmitToMerchant(Guid id, CancellationToken ct) // Endpoint مساعد للطلبات القديمة فقط، ويمكن حذفه لاحقاً إذا صار POST يرسل الطلب دائماً للمطعم
        //    => RunOrderChange(() => _orderWorkflowService.SubmitToMerchantAsync(id, ct));

        [HttpPatch("{id:guid}/approve-by-merchant")]
        public Task<IActionResult> ApproveByMerchant(Guid id, CancellationToken ct) // موافقة المطعم
            => RunOrderChange(() => _merchantDecisionService.ApproveAsync(id, ct));

        [HttpPatch("{id:guid}/reject-by-merchant")]
        public Task<IActionResult> RejectByMerchant(Guid id, [FromBody] MerchantDecisionRequest request, CancellationToken ct) // رفض المطعم مع السبب
            => RunOrderChange(() => _merchantDecisionService.RejectAsync(id, request, ct));

        [HttpPatch("{id:guid}/ready-for-pickup")]
        public Task<IActionResult> ReadyForPickup(Guid id, CancellationToken ct) // المطعم جهز الطلب
            => RunOrderChange(() => _orderWorkflowService.ReadyForPickupAsync(id, ct));

        [HttpPatch("{id:guid}/picked-up")]
        public Task<IActionResult> PickedUp(Guid id, CancellationToken ct) // السائق استلم الطلب
            => RunOrderChange(() => _orderWorkflowService.PickedUpAsync(id, ct));

        [HttpPatch("{id:guid}/on-the-way")]
        public Task<IActionResult> OnTheWay(Guid id, CancellationToken ct) // الطلب صار بالطريق
            => RunOrderChange(() => _orderWorkflowService.OnTheWayAsync(id, ct));

        [HttpPatch("{id:guid}/delivered")]
        public Task<IActionResult> Delivered(Guid id, CancellationToken ct) // الطلب وصل للزبون
            => RunOrderChange(() => _orderWorkflowService.DeliveredAsync(id, ct));

        [HttpPatch("{id:guid}/cancel")]
        public Task<IActionResult> Cancel(Guid id, [FromBody] CancelOrderRequest request, CancellationToken ct) // إلغاء الطلب
            => RunOrderChange(() => _cancelOrderService.ExecuteAsync(id, request, ct));

        [HttpPatch("{id:guid}/paid")]
        public Task<IActionResult> MarkAsPaid(Guid id, CancellationToken ct) // تعليم الطلب مدفوع
            => RunOrderChange(() => _orderPaymentService.MarkAsPaidAsync(id, ct));

        [HttpPatch("{id:guid}/unpaid")]
        public Task<IActionResult> MarkAsUnpaid(Guid id, CancellationToken ct) // تعليم الطلب غير مدفوع
            => RunOrderChange(() => _orderPaymentService.MarkAsUnpaidAsync(id, ct));

        private static async Task<IActionResult> RunOrderChange(Func<Task<bool>> change) // دالة مشتركة لتوحيد ردود تعديل حالة الطلب
        {
            try
            {
                var changed = await change(); // true يعني الطلب موجود وتعدل
                return changed ? new NoContentResult() : new NotFoundResult();
            }
            catch (DomainException ex)
            {
                return new BadRequestObjectResult(new { ex.Code, ex.Message });
            }
        }
    }
}
