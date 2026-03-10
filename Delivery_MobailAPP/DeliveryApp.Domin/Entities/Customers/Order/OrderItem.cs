using System;
using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.DomainExceptions;

namespace DeliveryApp.Domain.Entities.Customers.Order
{
    public class OrderItem
    {
        public OrderItemID ID { get; private set; }
        public OrderID OrderID { get; private set; }

        public string ProductNameSnapshot { get; private set; } = null!;
        public string? VariantNameSnapshot { get; private set; }

        public decimal UnitPriceSnapshot { get; private set; }
        public int Quantity { get; private set; }

        public decimal LineTotalSnapshot { get; private set; }

        public string? CustomerNote { get; private set; }

        private OrderItem() { }

        public OrderItem(OrderItemID id, OrderID OrderId, string productName, string? variantName,
            decimal unitPrice, int quantity, string? customerNote)
        {
            if (id.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(id));

            if (OrderId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(OrderId));

            ID = id;
            OrderID = OrderId;

            SetProductName(productName);
            SetVariantName(variantName);

            SetUnitPrice(unitPrice);
            SetQuantity(quantity);

            LineTotalSnapshot = UnitPriceSnapshot * Quantity;

            SetCustomerNote(customerNote);
        }

        // -------------------------
        //         Setters
        // -------------------------

        private void SetProductName(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(ProductNameSnapshot));

            value = value.Trim();

            if (value.Length > 150) throw new DomainValidationException
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, nameof(ProductNameSnapshot));

            ProductNameSnapshot = value;
        }

        private void SetVariantName(string? value)
        {
            value = NormalizeOptional(value);

            if (value is not null && value.Length > 150) throw new DomainValidationException
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, nameof(VariantNameSnapshot));

            VariantNameSnapshot = value;
        }

        private void SetUnitPrice(decimal value)
        {
            if (value < 0) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(UnitPriceSnapshot));

            UnitPriceSnapshot = value;
        }

        private void SetQuantity(int value)
        {
            if (value < 1) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(Quantity));

            Quantity = value;
        }

        private void SetCustomerNote(string? value)
        {
            value = NormalizeOptional(value);

            if (value is not null && value.Length > 500) throw new DomainValidationException
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, nameof(CustomerNote));

            CustomerNote = value;
        }

        private static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}