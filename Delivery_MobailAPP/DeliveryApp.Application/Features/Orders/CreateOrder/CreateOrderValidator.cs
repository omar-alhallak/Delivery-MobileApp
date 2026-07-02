namespace DeliveryApp.Application.Features.Orders.CreateOrder
{
    public static class CreateOrderValidator // Validation الخاص بطلب الإنشاء
    {
        public static CreateOrderRequest Validate(CreateOrderRequest request) // فحص البيانات الأساسية قبل إنشاء كائن Order
        {
            if (request is null) throw new Exception("Order request is required.");
            if (request.Items is null || request.Items.Count == 0) throw new Exception("Order items are required.");

            return request;
        }
    }
}