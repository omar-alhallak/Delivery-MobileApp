using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.DomainExceptions;

namespace DeliveryApp.Domain.Entities.Customers.Order
{
    public class OrderItem // يمثل عنصر داخل الطلب (Line Item)
    {
        // -------------------------
        //            Key
        // -------------------------

        public OrderItemID ID { get; private set; } // pk معرف العنصر
        public OrderID OrderID { get; private set; } // الطلب المرتبط به

        // -------------------------
        //         Snapshot
        // -------------------------

        public string ProductNameSnapshot { get; private set; } = null!; // اسم المنتج وقت الطلب
        public string? VariantNameSnapshot { get; private set; } // اسم التفصيل (مثل الحجم) وقت الطلب

        public decimal UnitPriceSnapshot { get; private set; } // سعر الوحدة وقت الطلب
        public int Quantity { get; private set; } // الكمية المطلوبة

        public decimal LineTotalSnapshot { get; private set; } // الإجمالي

        public string? CustomerNote { get; private set; } // ملاحظة الزبون على العنصر

        private OrderItem() { }

        public OrderItem(OrderItemID id, OrderID orderId, string productName, string? variantName, decimal unitPrice, int quantity, string? customerNote)
        {
            if (id.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(id));

            if (orderId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(orderId));

            ID = id;
            OrderID = orderId;

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

        private void SetProductName(string value) // إدخال اسم المنتج
        {
            if (string.IsNullOrWhiteSpace(value)) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(ProductNameSnapshot));

            value = value.Trim();

            if (value.Length > 150) throw new DomainValidationException
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, nameof(ProductNameSnapshot));

            ProductNameSnapshot = value;
        }

        private void SetVariantName(string? value) // إدخال اسم التفصيل 
        {
            value = NormalizeOptional(value);

            if (value is not null && value.Length > 150) throw new DomainValidationException
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, nameof(VariantNameSnapshot));

            VariantNameSnapshot = value;
        }

        private void SetUnitPrice(decimal value) // إدخال سعر الوحدة
        {
            if (value <= 0) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(UnitPriceSnapshot));

            UnitPriceSnapshot = value;
        }

        private void SetQuantity(int value) // إدخال الكمية
        {
            if (value < 1) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(Quantity));

            Quantity = value;
        }

        private void SetCustomerNote(string? value) // إدخال ملاحظة الزبون
        {
            value = NormalizeOptional(value);

            if (value is not null && value.Length > 500) throw new DomainValidationException
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, nameof(CustomerNote));

            CustomerNote = value;
        }

        private static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim(); // تنظيف النصوص
    }
}