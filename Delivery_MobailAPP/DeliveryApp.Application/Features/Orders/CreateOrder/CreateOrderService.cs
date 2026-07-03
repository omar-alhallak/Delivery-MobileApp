using DeliveryApp.Application.Interfaces.Services;
using DeliveryApp.Domain.Entities.Customers.Orders;
using DeliveryApp.Application.Features.Orders.Common;
using DeliveryApp.Application.Interfaces.OrderRepositoresInterfaces;
using DeliveryApp.Domain.Enums.OrderEnums;

namespace DeliveryApp.Application.Features.Orders.CreateOrder
{
    public sealed class CreateOrderService // Use case مسؤول عن إنشاء طلب جديد وإرساله مباشرة للمطعم
    {
        private const decimal FixedDeliveryFee = 20_000m;

        private readonly IOrderCreateRepository _repository; 
        private readonly IPublicCodeGenerator _codeGenerator;

        public CreateOrderService(IOrderCreateRepository repository, IPublicCodeGenerator codeGenerator)
        {
            _repository = repository;
            _codeGenerator = codeGenerator;
        }

        public async Task<OrderDto> ExecuteAsync(Guid currentUserId, CreateOrderRequest request, CancellationToken ct = default) // تنفيذ سيناريو إنشاء الطلب
        {
            var input = CreateOrderValidator.Validate(request); // فحص البيانات القادمة من الواجهة
            var now = DateTimeOffset.UtcNow; // وقت الإنشاء الرسمي
            var orderId = OrderID.New(); // إنشاء ID داخلي جديد للطلب
            var customerId = UserID.From(currentUserId);
            var merchantId = MerchantID.From(input.MerchantId);
            var addressId = AddressID.From(input.AddressId);

            if (!await _repository.IsActiveMerchantAsync(merchantId, ct))
                throw new DeliveryApp.Domain.DomainExceptions.DomainValidationException(
                    "Order.Merchant_Inactive_Or_Missing",
                    "Merchant is inactive or missing.",
                    nameof(input.MerchantId));

            if (!await _repository.IsCompletedCustomerAddressAsync(customerId, addressId, ct))
                throw new DeliveryApp.Domain.DomainExceptions.DomainValidationException(
                    "Order.Address_Invalid",
                    "Address must belong to the customer and be active and completed.",
                    nameof(input.AddressId));

            var location = await _repository.GetLocationAsync(
                customerId,
                merchantId,
                addressId,
                ct);

            if (location is null)
                throw new DeliveryApp.Domain.DomainExceptions.DomainValidationException(
                    "Order.Address_Or_Merchant_Invalid",
                    "Active merchant and completed customer address are required.",
                    nameof(input.AddressId));

            var orderItems = new List<OrderItem>();
            decimal itemsTotal = 0;

            foreach (var item in input.Items)
            {
                var catalogItem = await _repository.GetCatalogItemAsync(
                    merchantId,
                    ProductID.From(item.ProductId),
                    item.VariantId.HasValue ? VariantID.From(item.VariantId.Value) : null,
                    ct);

                if (catalogItem is null)
                    throw new DeliveryApp.Domain.DomainExceptions.DomainValidationException(
                        "Order.Product_Unavailable",
                        "Product or variant is unavailable for this merchant.",
                        nameof(item.ProductId));

                orderItems.Add(new OrderItem(
                    OrderItemID.New(),
                    orderId,
                    catalogItem.ProductName,
                    catalogItem.VariantName,
                    catalogItem.UnitPrice,
                    item.Quantity,
                    item.CustomerNote));

                itemsTotal += catalogItem.UnitPrice * item.Quantity;
            }

            var distanceKm = CalculateDistanceKm(
                location.PickupLatitude,
                location.PickupLongitude,
                location.DropoffLatitude,
                location.DropoffLongitude);

            // إنشاء كائن Order نفسه مع كل القيم المطلوبة من الدومين
            var order = new Order(
                orderId,
                input.OrderType,
                customerId,
                merchantId,
                location.PickupLatitude,
                location.PickupLongitude,
                location.DropoffLatitude,
                location.DropoffLongitude,
                distanceKm,
                itemsTotal,
                FixedDeliveryFee,
                input.TipAmount,
                itemsTotal + FixedDeliveryFee + input.TipAmount,
                PaymentMethod.Cash,
                PaymentStatus.UnPaid,
                1,
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

        private static double CalculateDistanceKm(double lat1, double lng1, double lat2, double lng2)
        {
            const double earthRadiusKm = 6371;
            var latitudeDifference = DegreesToRadians(lat2 - lat1);
            var longitudeDifference = DegreesToRadians(lng2 - lng1);

            var a = Math.Sin(latitudeDifference / 2) * Math.Sin(latitudeDifference / 2)
                + Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2))
                * Math.Sin(longitudeDifference / 2) * Math.Sin(longitudeDifference / 2);

            var distance = earthRadiusKm * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return Math.Round(distance, 2);
        }

        private static double DegreesToRadians(double degrees) => degrees * Math.PI / 180;
    }
}
