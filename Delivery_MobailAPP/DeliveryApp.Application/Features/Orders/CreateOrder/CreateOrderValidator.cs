namespace DeliveryApp.Application.Features.Orders.CreateOrder
{
    public static class CreateOrderValidator // Validation الخاص بطلب الإنشاء
    {
        public static CreateOrderRequest Validate(CreateOrderRequest request) // فحص البيانات الأساسية قبل إنشاء كائن Order
        {
            if (request is null)
                throw new DeliveryApp.Domain.DomainExceptions.DomainValidationException("Order.Request_Required", "Order request is required.", "request");

            if (request.MerchantId == Guid.Empty)
                throw new DeliveryApp.Domain.DomainExceptions.DomainValidationException("Order.Merchant_Required", "Merchant is required.", nameof(request.MerchantId));

            if (request.AddressId == Guid.Empty)
                throw new DeliveryApp.Domain.DomainExceptions.DomainValidationException("Order.Address_Required", "Address is required.", nameof(request.AddressId));

            if (request.Items is null || request.Items.Count == 0)
                throw new DeliveryApp.Domain.DomainExceptions.DomainValidationException("Order.Items_Required", "Order items are required.", nameof(request.Items));

            foreach (var item in request.Items)
            {
                if (item.ProductId == Guid.Empty)
                    throw new DeliveryApp.Domain.DomainExceptions.DomainValidationException("Order.Product_Required", "Product is required.", nameof(item.ProductId));

                if (item.VariantId == Guid.Empty)
                    throw new DeliveryApp.Domain.DomainExceptions.DomainValidationException("Order.Variant_Invalid", "Variant is invalid.", nameof(item.VariantId));

                if (item.Quantity < 1)
                    throw new DeliveryApp.Domain.DomainExceptions.DomainValidationException("Order.Quantity_Invalid", "Quantity must be at least one.", nameof(item.Quantity));
            }

            return request;
        }
    }
}
