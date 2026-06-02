namespace DeliveryApp.Application.Features.Orders.CancelOrder
{
    public static class CancelOrderValidator // Validation الخاص بإلغاء الطلب
    {
        public static CancelOrderRequest Validate(CancelOrderRequest request) // التأكد أن بيانات الإلغاء موجودة
        {
            if (request is null) throw new Exception("Cancel request is required.");
            return request;
        }
    }
}