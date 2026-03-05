using System;
using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.ValueObjects;
using DeliveryApp.Domain.DomainExceptions;

namespace DeliveryApp.Domain.Entities.Merchants.Catalog
{
    public class Variant
    {
        public VariantID ID { get; private set; }
        public ProductID ProductID { get; private set; }

        public CatalogName VariantName { get; private set; } = null!;

        public decimal BasePrice { get; private set; }

        public bool IsActive { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }

        private Variant() { }

        public Variant(VariantID Id, ProductID ProductId, string variantName, decimal basePrice, DateTimeOffset CreatedAtUtc)
        {
            if (Id.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(ID));

            if (ProductId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(ProductID));

            ID = Id;
            ProductID = ProductId;

            SetName(variantName);
            SetBasePrice(basePrice);

            IsActive = true;
            CreatedAt = CreatedAtUtc;
        }

        // -------------------------
        //          Behavior
        // -------------------------

        public void Rename(string name) => SetName(name);

        public void ChangePrice(decimal basePrice) => SetBasePrice(basePrice);

        public void Activate() => IsActive = true;

        public void Deactivate() => IsActive = false;

        // -------------------------
        //           Setters
        // -------------------------

        private void SetName(string value) =>
            VariantName = CatalogName.Create(value, maxLength: 100, field: nameof(VariantName));

        private void SetBasePrice(decimal value)
        {
            if (value <= 0) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(BasePrice));

            BasePrice = value;
        }
    }
}