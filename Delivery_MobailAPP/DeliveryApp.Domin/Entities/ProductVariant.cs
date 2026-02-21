using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Domain.Entities
{
    internal class ProductVariant
    {
        public Guid VariantId { get; private set; }


        public Guid ProductId { get; private set; }
        public Product ?Product { get; private set; }


        public string ?VariantName { get; private set; }

        public decimal BasePrice { get; private set; }


        public bool IsActive { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; }

        private ProductVariant() { }

        // الكونسرتكتور الأساسي
        public ProductVariant(Guid productId, string variantName, decimal basePrice)
        {
            VariantId = Guid.NewGuid(); // استخدام الأصل
            CreatedAt = DateTimeOffset.UtcNow;
            IsActive = true;

            UpdateDetails(productId, variantName, basePrice);
        }

        public void UpdateDetails(Guid productId, string variantName, decimal basePrice)
        {
            if (productId == Guid.Empty) throw new ArgumentException("ProductId is required");
            if (string.IsNullOrWhiteSpace(variantName)) throw new ArgumentException("Variant name cannot be empty");
            if (basePrice < 0) throw new ArgumentException("Price cannot be negative");

            ProductId = productId;
            VariantName = variantName.Trim();
            BasePrice = basePrice;
        }

        public void SetStatus(bool isActive) => IsActive = isActive;
    }
}

