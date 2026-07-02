using DeliveryApp.Application.Interfaces.Services;
using DeliveryApp.Domain.Entities.Customers.Orders;
using DeliveryApp.Application.Features.Orders.Common;
using DeliveryApp.Application.Interfaces.OrderRepositoresInterfaces;

namespace DeliveryApp.Application.Features.Orders.CreateOrder
{
    public sealed class CreateOrderService // Use case مسؤول عن إنشاء طلب جديد وإرساله مباشرة للمطعم
    {
        private readonly IOrderCreateRepository _repository; 
        private readonly IPublicCodeGenerator _codeGenerator;

        public CreateOrderService(IOrderCreateRepository repository, IPublicCodeGenerator codeGenerator)
        {
            _repository = repository;
            _codeGenerator = codeGenerator;
        }

        public async Task<OrderDto> ExecuteAsync(CreateOrderRequest request, CancellationToken ct = default) // تنفيذ سيناريو إنشاء الطلب
        {
            var input = CreateOrderValidator.Validate(request); // فحص البيانات القادمة من الواجهة
            var now = DateTimeOffset.UtcNow; // وقت الإنشاء الرسمي
            var orderId = OrderID.New(); // إنشاء ID داخلي جديد للطلب

            // تحويل المنتجات القادمة من الواجهة إلى OrderItem داخل الدومين
            var orderItems = input.Items.Select(item => new OrderItem(
                OrderItemID.New(),
                orderId,
                item.ProductName,
                item.VariantName,
                item.UnitPrice,
                item.Quantity,
                item.CustomerNote));

            // إنشاء كائن Order نفسه مع كل القيم المطلوبة من الدومين
            var order = new Order(
                orderId,
                input.OrderType,
                UserID.From(input.CustomerId),
                input.MerchantId.HasValue ? MerchantID.From(input.MerchantId.Value) : null,
                input.PickupLatitude,
                input.PickupLongitude,
                input.DropoffLatitude,
                input.DropoffLongitude,
                input.DistanceKm,
                input.ItemsTotal,
                input.DeliveryFee,
                input.TipAmount,
                input.ItemsTotal + input.DeliveryFee + input.TipAmount,
                input.PaymentMethod,
                input.PaymentStatus,
                input.RequiredDriversCount,
                orderItems,
                now);

            var publicId = await _codeGenerator.GenerateOrderCodeAsync(ct); // توليد رقم ظاهر للمستخدم
            order.AssignPublicID(publicId); // ربط الرقم الظاهر مع الطلب

            // حالياً تأكيد المستخدم يعني إرسال الطلب مباشرة للمطعم.
            // نمر بهذه الخطوات داخلياً لأن الدومين لا يسمح بالقفز مباشرة من Draft إلى AwaitingMerchantApproval.
            order.StartSearchingDrivers();
            order.ConfirmDrivers();
            order.MoveToAwaitingMerchantApproval();

            await _repository.AddOrderAsync(order, ct); // تجهيز الطلب للإضافة بقاعدة البيانات
            await _repository.SaveChangesAsync(ct); // تنفيذ الحفظ فعلياً

            return OrderMapper.ToDto(order, []); // إرجاع نسخة مناسبة للـ API بدل كائن الدومين
        }
    }
}