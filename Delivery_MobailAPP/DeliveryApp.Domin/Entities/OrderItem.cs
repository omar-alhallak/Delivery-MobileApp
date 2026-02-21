using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Domain.Entities
{
    internal class OrderItem
    {

        public Guid OrderItemId { get; private set; }



        public Guid OrderId { get; private set; }




        public decimal ProductNameSnapshot { get; private set; }

        public decimal VariantNamePriceSnapshot { get; private set; }


        public decimal UnitPriceSnapShot { get; private set; }



        public int Quantity { get; private set; }


        public decimal LineTotalSnapShot { get; private set; }


        public string? CustomerNotes { get; private set; }


        public OrderItem(Guid orderId, Guid productId, decimal productName, decimal unitPrice, int quantity, decimal variantName , string? notes = null)
        {
            OrderItemId = Guid.NewGuid();
            OrderId = orderId;
            if (CustomerNotes is not null && CustomerNotes.Length > 200)
                throw new ArgumentException("CustomerNotes is too long.");
            if (ProductNameSnapshot <= 0) throw new ArgumentException("Price must be greater than zero");
            if (VariantNamePriceSnapshot <= 0) throw new ArgumentException("Price must be greater than zero");
            if (UnitPriceSnapShot <= 0) throw new ArgumentException("Price must be greater than zero");
            if (LineTotalSnapShot <= 0) throw new ArgumentException("Price must be greater than zero");

            ProductNameSnapshot = productName;
            VariantNamePriceSnapshot = variantName;
            UnitPriceSnapShot = unitPrice;
            Quantity = quantity;

            LineTotalSnapShot = unitPrice * quantity;

            CustomerNotes = notes;
        }
        public void UpdateQuantityAndPrice(int quantity, decimal unitPrice)
        {
            // التأكد من الكمية (لا يمكن طلب 0 أو سالب)
            if (quantity <= 0)
                throw new ArgumentException("The quantity must be at least 1");

            // التأكد من السعر (لقطة السعر وقت الشراء)
            if (unitPrice < 0)
                throw new ArgumentException("The unit price cannot be negative.");

            this.Quantity = quantity;
            this.UnitPriceSnapShot = unitPrice;

            // الحساب التلقائي للإجمالي لضمان عدم وجود تلاعب
            this.LineTotalSnapShot = quantity * unitPrice;
        }

    }
}
